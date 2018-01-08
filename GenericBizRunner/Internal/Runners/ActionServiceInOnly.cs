// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOnly<TBizInterface, TBizIn> : ActionServiceBase
    {
        public ActionServiceInOnly(WriteToDbStates writeStates, IGenericBizRunnerConfig config)
            : base(writeStates, config)
        {
        }

        public void RunBizActionDbAndInstance(DbContext db, TBizInterface bizInstance, object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true, true, Config);
            var bizStatus = (IBizActionStatus)bizInstance;

            var inData = toBizCopier.DoCopyToBiz<TBizIn>(db, inputData);

            ((IGenericActionInOnlyAsync<TBizIn>)bizInstance).BizActionAsync(inData);

            //This handles optional call of save changes
            SaveChangedIfRequiredAndNoErrors(db, bizStatus);
        }
    }
}