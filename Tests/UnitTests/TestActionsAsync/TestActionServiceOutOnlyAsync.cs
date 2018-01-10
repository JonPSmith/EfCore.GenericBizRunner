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
    public class TestActionServiceOutOnlyAsync
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };

        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());

        //Beacause this is ValueInOut then there is no need for a mapper, but the ActionService checks that the Mapper isn't null
        private readonly IMapper _mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();

        [Fact]
        public async Task TestActionServiceOutOnlyDirectOk()
        {
            //SETUP 
            var bizInstance = new BizActionOutOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionOutOnlyAsync>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);

            //ATTEMPT
            var data = await runner.RunBizActionAsync<BizDataOut>();

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }

        [Fact]
        public async Task TestActionServiceOutOnlyDtosAsyncOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDtoAsync, ServiceLayerBizOutDtoAsync>();
            var bizInstance = new BizActionOutOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionOutOnlyAsync>(_emptyDbContext, bizInstance, mapper, _noCachingConfig);

            //ATTEMPT
            var data = await runner.RunBizActionAsync<ServiceLayerBizOutDtoAsync>();

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }

        [Fact]
        public async Task TestActionServiceOutOnlyDtosDatabaseOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();
                var bizInstance = new BizActionOutOnlyWriteDbAsync(context);
                var runner =
                    new ActionServiceAsync<IBizActionOutOnlyWriteDbAsync>(context, bizInstance, mapper, _noCachingConfig);

                //ATTEMPT
                var data = await runner.RunBizActionAsync<ServiceLayerBizOutDto>();

                //VERIFY
                bizInstance.HasErrors.ShouldBeFalse();
                context.LogEntries.Single().LogText.ShouldEqual("BizActionOutOnlyWriteDbAsync");
            }
        }

        [Fact]
        public async Task TestActionServiceOutOnlyDtosOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();
            var bizInstance = new BizActionOutOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionOutOnlyAsync>(_emptyDbContext, bizInstance, mapper, _noCachingConfig);

            //ATTEMPT
            var data = await runner.RunBizActionAsync<ServiceLayerBizOutDto>();

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }

        [Fact]
        public async Task TestCallHasNoOutputBad()
        {
            //SETUP 
            var bizInstance = new BizActionOutOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionOutOnlyAsync>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = "string";

            //ATTEMPT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await runner.RunBizActionAsync(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionOutOnlyAsync needed 'In, Async' but the Business class had a different setup of 'Out, Async'");
        }

        //---------------------------------------------------------------
        //error checking

        [Fact]
        public async Task TestInputIsBad()
        {
            //SETUP 
            var bizInstance = new BizActionOutOnlyAsync();
            var runner = new ActionServiceAsync<IBizActionOutOnlyAsync>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = "string";

            //ATTEMPT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await runner.RunBizActionAsync<string>(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionOutOnlyAsync needed 'InOut, Async' but the Business class had a different setup of 'Out, Async'");
        }
    }
}