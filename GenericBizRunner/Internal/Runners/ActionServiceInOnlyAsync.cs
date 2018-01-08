// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOnlyAsync<TBizInterface, TBizIn> : ActionServiceBase
    {
        public ActionServiceInOnlyAsync(WriteToDbStates writeStates, IGenericBizRunnerConfig config)
            : base(writeStates, config)
        {
        }

        public async Task RunBizActionDbAndInstanceAsync(DbContext db, TBizInterface bizInstance, object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true, true, Config);
            var bizStatus = (IBizActionStatus)bizInstance;

            var inData = await toBizCopier.DoCopyToBizAsync<TBizIn>(db, inputData).ConfigureAwait(false);

            await ((IGenericActionInOnlyAsync<TBizIn>) bizInstance).BizActionAsync(inData).ConfigureAwait(false);

            //This handles optional call of save changes
            await SaveChangedIfRequiredAndNoErrorsAsync(db, bizStatus).ConfigureAwait(false);
        }
    }
}