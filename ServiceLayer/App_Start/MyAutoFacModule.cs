// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Autofac;
using BizDbAccess;
using BizLogic;
using GenericBizRunner;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer
{
    public class ServiceLayerModule : Autofac.Module //#A
    {
        protected override void Load( //#B
            ContainerBuilder builder) //#B
        {
            //Need to register DbContext pointing to the EfCoreContext
            builder.RegisterType<EfCoreContext>().As<DbContext>();

            //and register the GenericActions interfaces by hand as they are generic
            builder.RegisterGeneric(typeof(ActionService<>)).As(typeof(IActionService<>));
            //builder.RegisterGeneric(typeof(ActionServiceAsync<>)).As(typeof(IActionServiceAsync<>));

            //-----------------------------
            //Now register the other layers
            builder.RegisterModule(new BizDbAccessModule());
            builder.RegisterModule(new BizLogicModule());
        }
    }

}