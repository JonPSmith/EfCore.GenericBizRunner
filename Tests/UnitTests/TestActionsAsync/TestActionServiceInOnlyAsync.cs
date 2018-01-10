// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner;
using GenericBizRunner.Configuration;
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
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };

        //This action does not access the database, but the ActionServiceAsync checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
        readonly IMapper _mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOutDirectOk(int num, bool hasErrors)
        {
            //SETUP         
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
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
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
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
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDtoAsync>();
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, mapper, _noCachingConfig);
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

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOutDtosDatabaseOk(int num, bool hasErrors)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var bizInstance = new BizActionInOnlyWriteDbAsync(context);
                var runner =
                    new ActionServiceAsync<IBizActionInOnlyWriteDbAsync>(context, bizInstance, _mapper, _noCachingConfig);
                var input = new ServiceLayerBizInDto {Num = num};

                //ATTEMPT
                await runner.RunBizActionAsync(input);

                //VERIFY
                bizInstance.HasErrors.ShouldEqual(hasErrors);
                if (hasErrors)
                {
                    context.LogEntries.Any().ShouldBeFalse();
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
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
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
            var runner = new ActionServiceAsync<IBizActionInOnlyAsync>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = "string";

            //ATTEMPT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>( async () => await runner.RunBizActionAsync(input));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = String, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,String>");
        }
    }
}