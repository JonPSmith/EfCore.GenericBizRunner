// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using GenericBizRunner;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Extensions.AssertExtensions;
using GenericBizRunner.Configuration;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using ServiceLayer.OrderServices;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using TestSupport.EfHelpers;

namespace Tests.UnitTests.DependencyInjection
{
    public class TestNetCoreDi
    {
        [Fact]
        public void TestRegisterGenericBizRunnerBasicOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterGenericBizRunnerBasic<TestDbContext>(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));

            //VERIFY
            service.Count.ShouldEqual(4);
            service.Contains(new ServiceDescriptor(typeof(DbContext), typeof(TestDbContext), ServiceLifetime.Scoped), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IActionService<>), typeof(ActionService<>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IActionServiceAsync<>), typeof(ActionServiceAsync<>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IWrappedBizRunnerConfigAndMappings), typeof(WrappedBizRunnerConfigAndMappings), ServiceLifetime.Singleton), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestRegisterGenericBizRunnerMultiDbContextOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterGenericBizRunnerMultiDbContext(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));

            //VERIFY
            service.Count.ShouldEqual(3);
            service.Contains(new ServiceDescriptor(typeof(IActionService<,>), typeof(ActionService<,>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IActionServiceAsync<,>), typeof(ActionServiceAsync<,>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IWrappedBizRunnerConfigAndMappings), typeof(WrappedBizRunnerConfigAndMappings), ServiceLifetime.Singleton), new CheckDescriptor()).ShouldBeTrue();
        }


        [Fact]
        public void TestRegisterGenericBizRunnerBothWithConfigOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterGenericBizRunnerBasic<TestDbContext>(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));
            service.RegisterGenericBizRunnerMultiDbContext(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));

            //VERIFY
            service.Count.ShouldEqual(6);
            service.Contains(new ServiceDescriptor(typeof(IWrappedBizRunnerConfigAndMappings), typeof(WrappedBizRunnerConfigAndMappings), ServiceLifetime.Singleton), new CheckDescriptor()).ShouldBeTrue();
        }

        //---------------------------------------------------
        //Check mappings found

        [Fact]
        public void TestRegisterGenericBizRunnerDtosFoundOk()
        {
            //SETUP
            var service = new ServiceCollection();
            DbContext emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
            service.RegisterGenericBizRunnerBasic<TestDbContext>(Assembly.GetAssembly(typeof(ServiceLayerBizInDto)));
            var diProvider = service.BuildServiceProvider();

            //ATTEMPT
            var wrappedConfig = diProvider.GetRequiredService<IWrappedBizRunnerConfigAndMappings>();
            var bizInstance = new BizActionInOut();
            var runner = new ActionService<IBizActionInOut>(emptyDbContext, bizInstance, wrappedConfig);
            var input = new ServiceLayerBizInDto { Num = 123 };
            var result = runner.RunBizAction<ServiceLayerBizOutDto>(input);

            //VERIFY
            runner.Status.HasErrors.ShouldBeFalse();
            result.Output.ShouldEqual("123");
        }

        //---------------------------------------------------

        private class CheckDescriptor : IEqualityComparer<ServiceDescriptor>
        {
            public bool Equals(ServiceDescriptor x, ServiceDescriptor y)
            {
                return x.ServiceType == y.ServiceType
                       && x.ImplementationType == y.ImplementationType || x.ImplementationType == null //null if use lambda
                       && x.Lifetime == y.Lifetime;
            }

            public int GetHashCode(ServiceDescriptor obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}