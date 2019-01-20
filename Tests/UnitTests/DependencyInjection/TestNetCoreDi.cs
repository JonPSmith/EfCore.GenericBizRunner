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
using ServiceLayer.OrderServices;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using TestSupport.EfHelpers;

namespace Tests.UnitTests.DependencyInjection
{
    public class TestNetCoreDi
    {
        [Fact]
        public void TestRegisterBizRunnerBasicOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterBizRunnerWithDtoScans<TestDbContext>(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));

            //VERIFY
            service.Count.ShouldEqual(4);
            service.Contains(new ServiceDescriptor(typeof(DbContext), typeof(TestDbContext), ServiceLifetime.Scoped), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IActionService<>), typeof(ActionService<>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IActionServiceAsync<>), typeof(ActionServiceAsync<>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IWrappedBizRunnerConfigAndMappings), typeof(WrappedBizRunnerConfigAndMappings), ServiceLifetime.Singleton), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestRegisterBizRunnerMultiDbContextOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterBizRunnerMultiDbContextWithDtoScans(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));

            //VERIFY
            service.Count.ShouldEqual(3);
            service.Contains(new ServiceDescriptor(typeof(IActionService<,>), typeof(ActionService<,>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IActionServiceAsync<,>), typeof(ActionServiceAsync<,>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IWrappedBizRunnerConfigAndMappings), typeof(WrappedBizRunnerConfigAndMappings), ServiceLifetime.Singleton), new CheckDescriptor()).ShouldBeTrue();
        }


        [Fact]
        public void TestRegisterBizRunnerBothWithConfigOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterBizRunnerWithDtoScans<TestDbContext>(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));
            service.RegisterBizRunnerMultiDbContextWithDtoScans(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));

            //VERIFY
            service.Count.ShouldEqual(6);
            service.Contains(new ServiceDescriptor(typeof(IWrappedBizRunnerConfigAndMappings), typeof(WrappedBizRunnerConfigAndMappings), ServiceLifetime.Singleton), new CheckDescriptor()).ShouldBeTrue();
        }

        //-----------------------------------------------------------------
        //errors and null

        [Fact]
        public void TestRegisterBizRunnerNoAssembliesToScan()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            var ex = Assert.Throws<ArgumentException>(() => service.RegisterBizRunnerWithDtoScans<TestDbContext>());

            //VERIFY
            ex.Message.ShouldStartWith("Needs assemblies to scan for DTOs. If not using DTOs just supply (Assembly)null as parameter.");
        }

        [Fact]
        public void TestRegisterBizRunnerNullToNotScan()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterBizRunnerWithDtoScans<TestDbContext>((Assembly)null);

            //VERIFY
            service.Count.ShouldEqual(4);
        }

        //---------------------------------------------------
        //Check mappings found

        [Fact]
        public void TestRegisterBizRunnerDtosFoundOk()
        {
            //SETUP
            var service = new ServiceCollection();
            DbContext emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
            service.RegisterBizRunnerWithDtoScans<TestDbContext>(Assembly.GetAssembly(typeof(ServiceLayerBizInDto)));
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