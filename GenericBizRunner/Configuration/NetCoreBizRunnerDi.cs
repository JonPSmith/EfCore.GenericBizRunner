// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericBizRunner.Configuration
{
    public static class NetCoreBizRunnerDi
    {
        /// <summary>
        /// This is the method for registering GeneriBizRunner with .NET Core DI provider
        /// </summary>
        /// <typeparam name="TDefaultDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void RegisterGenericBizRunnerBasic<TDefaultDbContext>(this IServiceCollection services, IGenericBizRunnerConfig config = null)
            where TDefaultDbContext : DbContext
        {
            services.AddScoped<DbContext>(sp => sp.GetService<TDefaultDbContext>());
            services.AddTransient(typeof(IActionService<>), typeof(ActionService<>));
            services.AddTransient(typeof(IActionServiceAsync<>), typeof(ActionServiceAsync<>));


            //Register the GenericBizRunnerConfig if given
            if (config != null)
                services.AddSingleton(config);
        }

        public static void RegisterGenericBizRunnerMultiDbContext(this IServiceCollection services, IGenericBizRunnerConfig config = null)
        {
            services.AddTransient(typeof(IActionService<,>), typeof(ActionService<,>));
            services.AddTransient(typeof(IActionServiceAsync<,>), typeof(ActionServiceAsync<,>));

            //Register the GenericBizRunnerConfig if given and not already registered
            if (config != null && 
                    !services.Contains(
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