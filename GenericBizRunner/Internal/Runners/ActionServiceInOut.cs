using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOut<TBizInterface, TBizIn, TBizOut> : ActionServiceBase
    {
        public ActionServiceInOut(WriteToDbStates writeStates, IGenericBizRunnerConfig config) : base(writeStates, config)
        {
        }

        public TOut RunBizActionDbAndInstance<TOut>(DbContext db, TBizInterface bizInstance, object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true);
            var fromBizCopier = DtoAccessGenerator.BuildCopier(typeof(TBizOut), typeof(TOut), false);
            var bizStatus = (IBizActionStatus) bizInstance;

            var status = toBizCopier.DoCopyToBiz<TBizIn>(db, inputData);
            if (bizStatus.HasErrors) return default(TOut);

            var result = ((IGenericAction<TBizIn, TBizOut>) bizInstance).RunAction(status.Result);

            //This handles optional call of save changes
            SaveChangedIfRequired(db, bizStatus, inputData, toBizCopier);
            if (bizStatus.HasErrors) return default(TOut);

            var data = fromBizCopier.DoCopyFromBiz<TOut>(db, result);
            return data;
        }

    }
}