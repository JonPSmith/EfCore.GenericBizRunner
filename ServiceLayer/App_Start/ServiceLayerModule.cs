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
    public class ServiceLayerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            //-----------------------------
            //Now register the other layers
            builder.RegisterModule(new BizDbAccessModule());
            builder.RegisterModule(new BizLogicModule());
        }
    }

}