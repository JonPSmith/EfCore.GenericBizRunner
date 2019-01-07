// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
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

namespace Tests.UnitTests.TestActions
{
    public class TestActionServiceOutOnly
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };

        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());

        //Beacause this is ValueInOut then there is no need for a mapper, but the ActionService checks that the Mapper isn't null
        private readonly IMapper _mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();

        [Fact]
        public void TestActionServiceOutOnlyDirectOk()
        {
            //SETUP 
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);

            //ATTEMPT
            var data = runner.RunBizAction<BizDataOut>();

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }

        [Fact]
        public void TestActionServiceOutOnlyDtosDatabaseOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();
                var bizInstance = new BizActionOutOnlyWriteDb(context);
                var runner =
                    new ActionService<IBizActionOutOnlyWriteDb>(context, bizInstance, mapper, _noCachingConfig);

                //ATTEMPT
                var data = runner.RunBizAction<ServiceLayerBizOutDto>();

                //VERIFY
                bizInstance.HasErrors.ShouldBeFalse();
                context.LogEntries.Single().LogText.ShouldEqual("BizActionOutOnlyWriteDb");
            }
        }

        [Fact]
        public void TestActionServiceOutOnlyDtosOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, mapper, _noCachingConfig);

            //ATTEMPT
            var data = runner.RunBizAction<ServiceLayerBizOutDto>();

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }

        [Fact]
        public void TestCallHasNoOutputBad()
        {
            //SETUP 
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionOutOnly needed 'In' but the Business class had a different setup of 'Out'");
        }

        //---------------------------------------------------------------
        //error checking

        [Fact]
        public void TestInputIsBad()
        {
            //SETUP 
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>( () => runner.RunBizAction<string>(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionOutOnly needed 'InOut' but the Business class had a different setup of 'Out'");
        }

        [Fact]
        public void TestUseAsyncBizInWithSyncBad()
        {
            //SETUP 
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction<ServiceLayerBizOutDtoAsync>());

            //VERIFY
            ex.Message.ShouldEqual("You cannot use an Async version of the DTO in a non-async action.");
        }
    }
}