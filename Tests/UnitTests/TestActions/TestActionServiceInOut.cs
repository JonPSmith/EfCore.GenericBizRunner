// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using AutoMapper;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using GenericBizRunner.PublicButHidden;
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

namespace Tests.UnitTests.TestActions
{
    public class TestActionServiceInOut
    {
        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
        private readonly IWrappedBizRunnerConfigAndMappings _wrappedConfig;

        public TestActionServiceInOut()
        {
            var config = new GenericBizRunnerConfig { TurnOffCaching = true };
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
            utData.AddDtoMapping<ServiceLayerBizOutDto>();
            _wrappedConfig = utData.WrappedConfig;
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceValueInOutDirectOk(int num, bool hasErrors)
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = num;

            //ATTEMPT
            var data = runner.RunBizAction<string>(input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
            if (hasErrors)
            {
                data.ShouldBeNull();
                bizInstance.Message.ShouldEqual("Failed with 1 error");
            }
            else
                data.ShouldEqual(num.ToString());
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOutDtosOk(int num, bool hasErrors)
        {
            //SETUP 
            var bizInstance = new BizActionInOut();
            var runner = new ActionService<IBizActionInOut>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = new ServiceLayerBizInDto{Num = num};

            //ATTEMPT
            var data = runner.RunBizAction<ServiceLayerBizOutDto>(input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
            if (hasErrors)
            {
                data.ShouldBeNull();
                bizInstance.Message.ShouldEqual("Failed with 1 error");
            }
            else
                data.Output.ShouldEqual(num.ToString());
        }

        [Fact]
        public void TestActionServiceErrorInSetupOk()
        {
            //SETUP         
            var bizInstance = new BizActionInOut();
            var runner = new ActionService<IBizActionInOut>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = runner.GetDto<ServiceLayerBizInDto>(x => { x.RaiseErrorInSetupSecondaryData = true; });

            //ATTEMPT
            runner.RunBizAction<ServiceLayerBizOutDto>(input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(true);
            bizInstance.Errors.Single().ErrorResult.ErrorMessage.ShouldEqual("Error in SetupSecondaryData");
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceValueInOutDatabaseOk(int num, bool hasErrors)
        {
            //SETUP 
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var bizInstance = new BizActionInOutWriteDb(context);
                var runner = new ActionService<IBizActionInOutWriteDb>(context, bizInstance, _wrappedConfig);
                var input = new ServiceLayerBizInDto { Num = num };

                //ATTEMPT
                var data = runner.RunBizAction<BizDataOut>(input);

                //VERIFY
                bizInstance.HasErrors.ShouldEqual(hasErrors);
                if (hasErrors)
                {
                    context.LogEntries.Any().ShouldBeFalse();
                    data.ShouldBeNull();
                }
                else
                {
                    context.LogEntries.Single().LogText.ShouldEqual(num.ToString());
                    data.Output.ShouldEqual(num.ToString());
                }
            }
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(1, true)]
        public void TestActionServiceDtosDatabaseValidationOk(int num, bool hasErrors)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var bizInstance = new BizActionInOnlyWriteDb(context);
                var runner =
                    new ActionService<IBizActionInOnlyWriteDb>(context, bizInstance, _wrappedConfig);
                var input = new ServiceLayerBizInDto { Num = num };

                //ATTEMPT
                runner.RunBizAction(input);

                //VERIFY
                bizInstance.HasErrors.ShouldEqual(hasErrors);
                if (hasErrors)
                {
                    context.LogEntries.Any().ShouldBeFalse();
                    input.SetupSecondaryDataCalled.ShouldBeTrue();
                }
                else
                {
                    context.LogEntries.Single().LogText.ShouldEqual(num.ToString());
                }
            }
        }

        [Fact]
        public void TestCallHasNoInputBad()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _wrappedConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction<string>());

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionValueInOut needed 'Out' but the Business class had a different setup of 'InOut'");
        }

        [Fact]
        public void TestCallHasNoOutputBad()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionValueInOut needed 'In' but the Business class had a different setup of 'InOut'");
        }

        //---------------------------------------------------------------
        //error checking

        [Fact]
        public void TestInputIsBad()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>( () => runner.RunBizAction<string>(input));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = String, to type Int32. Expected a DTO of type GenericActionToBizDto<Int32,String>");
        }

        [Fact]
        public void TestUseAsyncBizInWithSyncBad()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = new ServiceLayerBizInDtoAsync();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction<string>(input));

            //VERIFY
            ex.Message.ShouldEqual("You cannot use an Async version of the DTO in a non-async action.");
        }
    }
}