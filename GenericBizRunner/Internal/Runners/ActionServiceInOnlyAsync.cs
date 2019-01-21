// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOnlyAsync<TBizInterface, TBizIn> : ActionServiceBase
    {
        public ActionServiceInOnlyAsync(bool requiresSaveChanges, IWrappedBizRunnerConfigAndMappings wrappedConfig)
            : base(requiresSaveChanges, wrappedConfig)
        {
        }

        public async Task RunBizActionDbAndInstanceAsync(DbContext db, TBizInterface bizInstance, object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true, true, WrappedConfig.Config.TurnOffCaching);
            var bizStatus = (IBizActionStatus)bizInstance;

            //The SetupSecondaryData produced errors
            if (bizStatus.HasErrors) return;

            var inData = await toBizCopier.DoCopyToBizAsync<TBizIn>(db, WrappedConfig.ToBizIMapper, inputData).ConfigureAwait(false);

            await ((IGenericActionInOnlyAsync<TBizIn>) bizInstance).BizActionAsync(inData).ConfigureAwait(false);

            //This handles optional call of save changes
            await SaveChangedIfRequiredAndNoErrorsAsync(db, bizStatus).ConfigureAwait(false);
            if (bizStatus.HasErrors)
                await toBizCopier.SetupSecondaryDataIfRequiredAsync(db, bizStatus, inputData).ConfigureAwait(false);
        }
    }
}