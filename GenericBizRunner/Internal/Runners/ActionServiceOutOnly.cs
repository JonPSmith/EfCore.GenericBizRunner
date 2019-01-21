// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using AutoMapper;
using GenericBizRunner.Configuration;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceOutOnly<TBizInterface, TBizOut> : ActionServiceBase
    {
        public ActionServiceOutOnly(bool requiresSaveChanges, IWrappedBizRunnerConfigAndMappings wrappedConfig)
            : base(requiresSaveChanges, wrappedConfig)
        {
        }

        public TOut RunBizActionDbAndInstance<TOut>(DbContext db, TBizInterface bizInstance)
        {
            var fromBizCopier = DtoAccessGenerator.BuildCopier(typeof(TBizOut), typeof(TOut), false, false, WrappedConfig.Config.TurnOffCaching);
            var bizStatus = (IBizActionStatus)bizInstance;

            var result = ((IGenericActionOutOnly<TBizOut>)bizInstance).BizAction();

            //This handles optional call of save changes
            SaveChangedIfRequiredAndNoErrors(db, bizStatus);
            if (bizStatus.HasErrors) return default(TOut);

            var data = fromBizCopier.DoCopyFromBiz<TOut>(db, WrappedConfig.FromBizIMapper, result);
            return data;
        }
    }
}