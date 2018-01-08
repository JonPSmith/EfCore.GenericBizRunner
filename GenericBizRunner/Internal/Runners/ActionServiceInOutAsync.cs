// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOutAsync<TBizInterface, TBizIn, TBizOut> : ActionServiceBase
    {
        public ActionServiceInOutAsync(WriteToDbStates writeStates, IGenericBizRunnerConfig config)
            : base(writeStates, config)
        {
        }

        public async Task<TOut> RunBizActionDbAndInstanceAsync<TOut>(DbContext db, TBizInterface bizInstance, object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true, true, Config);
            var fromBizCopier = DtoAccessGenerator.BuildCopier(typeof(TBizOut), typeof(TOut), false, true, Config);
            var bizStatus = (IBizActionStatus)bizInstance;

            var inData = await toBizCopier.DoCopyToBizAsync<TBizIn>(db, inputData).ConfigureAwait(false);

            var result = await ((IGenericActionAsync<TBizIn, TBizOut>)bizInstance).BizActionAsync(inData).ConfigureAwait(false);

            //This handles optional call of save changes
            SaveChangedIfRequiredAndNoErrors(db, bizStatus);
            if (bizStatus.HasErrors) return ReturnDefaultAndResetInDto<TOut>(db, toBizCopier, inputData);

            var data = await fromBizCopier.DoCopyFromBizAsync<TOut>(db, result).ConfigureAwait(false);
            return data;
        }
    }
}