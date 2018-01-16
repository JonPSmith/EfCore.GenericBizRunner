// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Autofac;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Configuration
{
    /// <summary>
    /// This is the AutoFac module that will register all the parts you need to use GenericBizRunner.
    /// You can register this in ASP.NET Core in the following ways
    /// <!-- ContainerBuilder.RegisterModule<GenericBizRunner.ConfigurationService.BizRunnerDiModule<YourDefaultDbContext>>(); -->
    /// Or if you want to set special GenericBizRunnerConfig, then you need to use this format
    /// <!-- ContainerBuilder.RegisterModule<GenericBizRunner.ConfigurationService.BizRunnerDiModule<YourDefaultDbContext>>{ -->
    /// <!--      SpecificConfig = YourGenericBizRunnerConfig}; -->
    /// </summary>
    /// <typeparam name="TDefaultDbContext">The type of your default DbContext. This will be used if you do not define what DbContext you want</typeparam>
    public class BizRunnerDiModule<TDefaultDbContext> : Autofac.Module
    {
        /// <summary>
        /// If you want to provide your own configuration for the GenericBizRunner then you should set this within the registering
        /// See comment above
        /// </summary>
        public IGenericBizRunnerConfig SpecificConfig { get; set; }

        /// <summary>
        /// This is the AutoFac Module's method for registering a Module
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            //Need to register DbContext pointing to the default DbContext that has been defined outside this module
            builder.Register(c => c.Resolve<TDefaultDbContext>()).As<DbContext>().InstancePerLifetimeScope();

            //and register the GenericActions interfaces by hand as they are generic
            builder.RegisterGeneric(typeof(ActionService<>)).As(typeof(IActionService<>));
            builder.RegisterGeneric(typeof(ActionServiceAsync<>)).As(typeof(IActionServiceAsync<>));

            //Now I register the other parts of the GenericBizRunner 
            builder.RegisterModule(new BizRunnerDiModuleBoundedContexts
            {
                SpecificConfig = SpecificConfig
            });

        }
    }
}