// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOnly<TBizInterface, TBizIn> : ActionServiceBase
    {
        public ActionServiceInOnly(bool requiresSaveChanges, IGenericBizRunnerConfig config)
            : base(requiresSaveChanges, config)
        {
        }

        public void RunBizActionDbAndInstance(DbContext db, TBizInterface bizInstance, IMapper mapper, object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true, false, Config);
            var bizStatus = (IBizActionStatus)bizInstance;

            //The SetupSecondaryData produced errors
            if (bizStatus.HasErrors) return;

            var inData = toBizCopier.DoCopyToBiz<TBizIn>(db, mapper, inputData);

            ((IGenericActionInOnly<TBizIn>)bizInstance).BizAction(inData);

            //This handles optional call of save changes
            SaveChangedIfRequiredAndNoErrors(db, bizStatus);
            if (bizStatus.HasErrors)
                toBizCopier.SetupSecondaryDataIfRequired(db, bizStatus, inputData);
        }
    }
}