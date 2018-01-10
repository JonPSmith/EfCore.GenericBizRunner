// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using Autofac;
using Autofac.Core.Registration;
using AutoMapper;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.DependecyInjection
{
    public class TestAutoFac
    {
        [Fact]
        public void TestCreateActionServiceNoConfig()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(options).SingleInstance();
            builder.RegisterType<TestDbContext>().As<DbContext>().InstancePerLifetimeScope();
            builder.RegisterType<BizActionInOut>().As<IBizActionInOut>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ActionService<>)).As(typeof(IActionService<>));
            builder.RegisterInstance(SetupHelpers.CreateMapper<ServiceLayerBizInDto>()).As<IMapper>().SingleInstance();
            //builder.RegisterInstance(new GenericBizRunnerConfig {TurnOffCaching = true}).As<IGenericBizRunnerConfig>();
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance = lifetimeScope.Resolve<IActionService<IBizActionInOut>>();
                Assert.NotNull(instance);
            }
        }

        [Fact]
        public void TestCreateActionServiceWithDbContextNoConfig()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(options).SingleInstance();
            builder.RegisterType<TestDbContext>().As<TestDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<BizActionInOut>().As<IBizActionInOut>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ActionService<,>)).As(typeof(IActionService<,>));
            builder.RegisterInstance(SetupHelpers.CreateMapper<ServiceLayerBizInDto>()).As<IMapper>().SingleInstance();
            //builder.RegisterInstance(new GenericBizRunnerConfig {TurnOffCaching = true}).As<IGenericBizRunnerConfig>();
            var container = builder.Build();

            //ATTEMPT & VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance = lifetimeScope.Resolve<IActionService<TestDbContext, IBizActionInOut>>();
                Assert.NotNull(instance);
            }
        }


        [Fact]
        public void TestGenericBizRunnerAutoFacModule()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(options).SingleInstance();
            builder.RegisterType<TestDbContext>().As<TestDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<BizActionInOut>().As<IBizActionInOut>().InstancePerLifetimeScope();
            builder.RegisterInstance(SetupHelpers.CreateMapper<ServiceLayerBizInDto>()).As<IMapper>().SingleInstance();

            //ATTEMPT
            builder.RegisterModule(new BizRunnerDiModule<TestDbContext>());
            var container = builder.Build();

            //VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance1 = lifetimeScope.Resolve<IActionService<IBizActionInOut>>();
                var instance2 = lifetimeScope.Resolve<IActionService<TestDbContext, IBizActionInOut>>();
                Assert.NotNull(instance1);
                Assert.NotNull(instance2);
            }
        }

        [Fact]
        public void TestGenericBizRunnerAutoFacModuleBoundedContext()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(options).SingleInstance();
            builder.RegisterType<TestDbContext>().As<TestDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<BizActionInOut>().As<IBizActionInOut>().InstancePerLifetimeScope();
            builder.RegisterInstance(SetupHelpers.CreateMapper<ServiceLayerBizInDto>()).As<IMapper>().SingleInstance();

            //ATTEMPT
            builder.RegisterModule(new BizRunnerDiModuleBoundedContexts());
            var container = builder.Build();

            //VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var instance = lifetimeScope.Resolve<IActionService<TestDbContext, IBizActionInOut>>();
                Assert.NotNull(instance);
            }
        }

        [Fact]
        public void TestGenericBizRunnerAutoFacModuleBoundedContextNoSingle()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(options).SingleInstance();
            builder.RegisterType<TestDbContext>().As<TestDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<BizActionInOut>().As<IBizActionInOut>().InstancePerLifetimeScope();
            builder.RegisterInstance(SetupHelpers.CreateMapper<ServiceLayerBizInDto>()).As<IMapper>().SingleInstance();

            //ATTEMPT
            builder.RegisterModule(new BizRunnerDiModuleBoundedContexts());
            var container = builder.Build();

            //VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var ex = Assert.Throws<ComponentNotRegisteredException>( ()  => lifetimeScope.Resolve<IActionService<IBizActionInOut>>());
                ex.Message.ShouldStartWith("The requested service 'GenericBizRunner.IActionService`1");
            }
        }

        [Fact]
        public void TestGenericBizRunnerAutoFacModuleAddConfig()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(options).SingleInstance();
            builder.RegisterType<TestDbContext>().As<TestDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<BizActionInOnlyWriteDb>().As<IBizActionInOnlyWriteDb>().InstancePerLifetimeScope();
            builder.RegisterInstance(SetupHelpers.CreateMapper<ServiceLayerBizInDto>()).As<IMapper>().SingleInstance();

            //ATTEMPT
            var config = new GenericBizRunnerConfig
            {
                DoNotValidateSaveChanges = true
            };
            builder.RegisterModule(new BizRunnerDiModule<TestDbContext>
            {
                SpecificConfig = config
            });
            var container = builder.Build();

            //VERIFY
            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var dbContext = lifetimeScope.Resolve<TestDbContext>();
                dbContext.Database.EnsureCreated();

                var runner = lifetimeScope.Resolve<IActionService<IBizActionInOnlyWriteDb>>();
                runner.RunBizAction(new BizDataIn{Num = 1});

                dbContext.LogEntries.Single().LogText.ShouldEqual("1");
            }
        }
    }
}