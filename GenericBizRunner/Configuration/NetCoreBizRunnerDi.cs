// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericBizRunner.Configuration
{
    /// <summary>
    /// This contains the code to register the GenericBizRunner library and also find and GenericBizRunner DTO and 
    /// </summary>
    public static class NetCoreBizRunnerDi
    {
        /// <summary>
        /// This is the method for registering GenericBizRunner and any GenericBizRunner DTOs with .NET Core DI provider
        /// </summary>
        /// <typeparam name="TDefaultDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="assembliesToScan">These are the assemblies to scan for DTOs</param>
        public static void RegisterGenericBizRunnerBasic<TDefaultDbContext>(this IServiceCollection services, params Assembly[] assembliesToScan)
            where TDefaultDbContext : DbContext
        {
            services.RegisterGenericBizRunnerBasic<TDefaultDbContext>(new GenericBizRunnerConfig(), assembliesToScan);
        }

        /// <summary>
        /// This is the method for registering GenericBizRunner and any GenericBizRunner DTOs with .NET Core DI provider with config
        /// </summary>
        /// <typeparam name="TDefaultDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="assembliesToScan">These are the assemblies to scan for DTOs</param>
        public static void RegisterGenericBizRunnerBasic<TDefaultDbContext>(this IServiceCollection services, IGenericBizRunnerConfig config,
            params Assembly[] assembliesToScan)
            where TDefaultDbContext : DbContext
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            services.AddScoped<DbContext>(sp => sp.GetService<TDefaultDbContext>());
            services.AddTransient(typeof(IActionService<>), typeof(ActionService<>));
            services.AddTransient(typeof(IActionServiceAsync<>), typeof(ActionServiceAsync<>));

            //Register the GenericBizRunnerConfig if given
            services.AddSingleton(config);
        }

        /// <summary>
        /// This is used to register GenericBizRunner and any GenericBizRunner DTOs with .NET Core DI provider to work with multiple DbContexts
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembliesToScan">These are the assemblies to scan for DTOs</param>
        public static void RegisterGenericBizRunnerMultiDbContext(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            services.RegisterGenericBizRunnerMultiDbContext(new GenericBizRunnerConfig(), assembliesToScan);
        }

        /// <summary>
        /// This is used to register GenericBizRunner and any GenericBizRunner DTOs with .NET Core DI provider to work with multiple DbContexts
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="assembliesToScan">These are the assemblies to scan for DTOs</param>
        public static void RegisterGenericBizRunnerMultiDbContext(this IServiceCollection services, IGenericBizRunnerConfig config,
            params Assembly[] assembliesToScan)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            services.AddTransient(typeof(IActionService<,>), typeof(ActionService<,>));
            services.AddTransient(typeof(IActionServiceAsync<,>), typeof(ActionServiceAsync<,>));

            //Register the GenericBizRunnerConfig if not already registered
            if (!services.Contains(
                    new ServiceDescriptor(typeof(IGenericBizRunnerConfig), config), new CheckDescriptor()))
            {
                services.AddSingleton(config);
            }
        }

        //---------------------------------------------------------
        //

        private class CheckDescriptor : IEqualityComparer<ServiceDescriptor>
        {
            public bool Equals(ServiceDescriptor x, ServiceDescriptor y)
            {
                return x.ServiceType == y.ServiceType
                       && x.ImplementationType == y.ImplementationType
                       && x.Lifetime == y.Lifetime;
            }

            public int GetHashCode(ServiceDescriptor obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}