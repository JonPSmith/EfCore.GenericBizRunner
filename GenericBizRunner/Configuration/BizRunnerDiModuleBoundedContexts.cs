// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Autofac;

namespace GenericBizRunner.Configuration
{
    /// <summary>
    /// This is the AutoFac module that will register all the parts you need to use GenericBizRunner
    /// when you using multiple DbContext's, known as bounded contexts. 
    /// You can register this in ASP.NET Core in the following way
    /// <!-- ContainerBuilder.RegisterModule<GenericBizRunner.ConfigurationService.BizRunnerDiModuleBoundedContexts>(); -->
    /// </summary>
    public class BizRunnerDiModuleBoundedContexts : Autofac.Module 
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

            //and register the GenericActions interfaces by hand, as they are generic
            //Here I only register the interfaces where you have to define the DbContext
            builder.RegisterGeneric(typeof(ActionService<,>)).As(typeof(IActionService<,>));
            builder.RegisterGeneric(typeof(ActionServiceAsync<,>)).As(typeof(IActionServiceAsync<,>));

            //Register the GenericBizRunnerConfig if given
            if (SpecificConfig != null)
                builder.RegisterInstance(SpecificConfig).As<IGenericBizRunnerConfig>();

        }
    }
}