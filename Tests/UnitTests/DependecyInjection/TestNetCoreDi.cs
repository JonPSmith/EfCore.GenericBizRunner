// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using GenericBizRunner;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Extensions.AssertExtensions;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.DbForTransactions;

namespace Tests.UnitTests.DependecyInjection
{
    public class TestNetCoreDi
    {
        [Fact]
        public void TestRegisterGenericBizRunnerBasicOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMP
            service.RegisterGenericBizRunnerBasic<TestDbContext>();

            //VERIFY
            service.Count.ShouldEqual(3);
            service.Contains(new ServiceDescriptor(typeof(DbContext), typeof(TestDbContext), ServiceLifetime.Scoped), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IActionService<>), typeof(ActionService<>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IActionServiceAsync<>), typeof(ActionServiceAsync<>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestRegisterGenericBizRunnerBasicWithConfigOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMP
            service.RegisterGenericBizRunnerBasic<TestDbContext>(new GenericBizRunnerConfig());

            //VERIFY
            service.Count.ShouldEqual(4);
            service.Contains(new ServiceDescriptor(typeof(IGenericBizRunnerConfig), typeof(GenericBizRunnerConfig), ServiceLifetime.Singleton), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestRegisterGenericBizRunnerMultiDbContextOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMP
            service.RegisterGenericBizRunnerMultiDbContext();

            //VERIFY
            service.Count.ShouldEqual(2);
            service.Contains(new ServiceDescriptor(typeof(IActionService<,>), typeof(ActionService<,>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IActionServiceAsync<,>), typeof(ActionServiceAsync<,>), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestRegisterGenericBizRunnerMultiDbContextWithConfigOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMP
            service.RegisterGenericBizRunnerMultiDbContext(new GenericBizRunnerConfig());

            //VERIFY
            service.Count.ShouldEqual(3);
            service.Contains(new ServiceDescriptor(typeof(IGenericBizRunnerConfig), typeof(GenericBizRunnerConfig), ServiceLifetime.Singleton), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestRegisterGenericBizRunnerBothWithConfigOk()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMP
            service.RegisterGenericBizRunnerBasic<TestDbContext>(new GenericBizRunnerConfig());
            service.RegisterGenericBizRunnerMultiDbContext(new GenericBizRunnerConfig());

            //VERIFY
            service.Count.ShouldEqual(6);
            service.Contains(new ServiceDescriptor(typeof(IGenericBizRunnerConfig), typeof(GenericBizRunnerConfig), ServiceLifetime.Singleton), new CheckDescriptor()).ShouldBeTrue();
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