// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.ActionsAsync;
using TestBizLayer.ActionsAsync.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestActionsAsync
{
    public class TestActionServiceInOnlyAsync
    {
        //This action does not access the database, but the ActionServiceAsync checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
        private readonly IWrappedBizRunnerConfigAndMappings _wrappedConfig;

        public TestActionServiceInOnlyAsync()
        {
            var config = new GenericBizRunnerConfig { TurnOffCaching = true };
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
            utData.AddDtoMapping<ServiceLayerBizOutDto>();
            utData.AddDtoMapping<ServiceLayerBizInDtoAsync>();
            _wrappedConfig = utData.WrappedConfig;
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOutDirectOk(int num, bool hasErrors)
        {
            //SETUP         
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = new BizDataIn { Num = num};

            //ATTEMPT
            await runner.RunBizActionAsync(input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
            if (hasErrors)
            {
                bizInstance.Message.ShouldEqual("Failed with 1 error");
            }
            else
                bizInstance.Message.ShouldEqual("All Ok");
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOutDtosOk(int num, bool hasErrors)
        {
            //SETUP         
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = new ServiceLayerBizInDto { Num = num };

            //ATTEMPT
            await runner.RunBizActionAsync(input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
            if (hasErrors)
            {
                bizInstance.Message.ShouldEqual("Failed with 1 error");
            }
            else
                bizInstance.Message.ShouldEqual("All Ok");
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOutDtosAsyncOk(int num, bool hasErrors)
        {
            //SETUP 
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = new ServiceLayerBizInDtoAsync { Num = num };

            //ATTEMPT
            await runner.RunBizActionAsync(input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
            if (hasErrors)
            {
                bizInstance.Message.ShouldEqual("Failed with 1 error");
            }
            else
                bizInstance.Message.ShouldEqual("All Ok");
        }

        [Fact]
        public async Task TestActionServiceErrorInSetupOk()
        {
            //SETUP         
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = await runner.GetDtoAsync<ServiceLayerBizInDto>(x => { x.RaiseErrorInSetupSecondaryData = true; });

            //ATTEMPT
            await runner.RunBizActionAsync(input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(true);
            bizInstance.Errors.Single().ErrorResult.ErrorMessage.ShouldEqual("Error in SetupSecondaryData");
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceDtosDatabaseOk(int num, bool hasErrors)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var bizInstance = new BizActionInOnlyWriteDbAsync(context);
                var runner =
                    new ActionServiceAsync<IBizActionInOnlyWriteDbAsync>(context, bizInstance, _wrappedConfig);
                var input = new ServiceLayerBizInDto {Num = num};

                //ATTEMPT
                await runner.RunBizActionAsync(input);

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

        [Theory]
        [InlineData(123, false)]
        [InlineData(1, true)]
        public async Task TestActionServiceDtosDatabaseValidationOk(int num, bool hasErrors)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var bizInstance = new BizActionInOnlyWriteDbAsync(context);
                var runner =
                    new ActionServiceAsync<IBizActionInOnlyWriteDbAsync>(context, bizInstance, _wrappedConfig);
                var input = new ServiceLayerBizInDto { Num = num };

                //ATTEMPT
                await runner.RunBizActionAsync(input);

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
        public async Task TestCallHasOutputBad()
        {
            //SETUP 
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = "string";

            //ATTEMPT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await runner.RunBizActionAsync<string>(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionInOnlyAsync needed 'InOut, Async' but the Business class had a different setup of 'In, Async'");
        }

        //---------------------------------------------------------------
        //error checking

        [Fact]
        public async Task TestInputIsBad()
        {
            //SETUP 
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _wrappedConfig);
            var input = "string";

            //ATTEMPT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>( async () => await runner.RunBizActionAsync(input));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = String, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,String>");
        }
    }
}