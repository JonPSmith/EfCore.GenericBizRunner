// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceOutOnlyAsync<TBizInterface, TBizOut> : ActionServiceBase
    {
        public ActionServiceOutOnlyAsync(WriteToDbStates writeStates, IGenericBizRunnerConfig config)
            : base(writeStates, config)
        {
        }

        public async Task<TOut> RunBizActionDbAndInstanceAsync<TOut>(DbContext db, TBizInterface bizInstance)
        {
            var fromBizCopier = DtoAccessGenerator.BuildCopier(typeof(TBizOut), typeof(TOut), false, true, Config);
            var bizStatus = (IBizActionStatus)bizInstance;

            var result = await ((IGenericActionOutOnlyAsync<TBizOut>)bizInstance).BizActionAsync().ConfigureAwait(false);

            //This handles optional call of save changes
            SaveChangedIfRequiredAndNoErrors(db, bizStatus);
            if (bizStatus.HasErrors) return default(TOut);

            var data = await fromBizCopier.DoCopyFromBizAsync<TOut>(db, result).ConfigureAwait(false);
            return data;
        }
    }
}