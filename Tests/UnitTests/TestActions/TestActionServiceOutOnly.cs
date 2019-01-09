// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestActions
{
    public class TestActionServiceOutOnly
    {  
        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
        private readonly IWrappedBizRunnerConfigAndMappings _wrappedConfig;

        public TestActionServiceOutOnly()
        {
            var config = new GenericBizRunnerConfig { TurnOffCaching = true };
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
            utData.AddDtoMapping<ServiceLayerBizOutDto>();
            _wrappedConfig = utData.WrappedConfig;
        }

        [Fact]
        public void TestActionServiceOutOnlyDirectOk()
        {
            //SETUP 
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, _wrappedConfig);

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
                var bizInstance = new BizActionOutOnlyWriteDb(context);
                var runner =
                    new ActionService<IBizActionOutOnlyWriteDb>(context, bizInstance, _wrappedConfig);

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
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, _wrappedConfig);

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
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, _wrappedConfig);
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
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, _wrappedConfig);
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
            var runner = new ActionService<IBizActionOutOnly>(_emptyDbContext, bizInstance, _wrappedConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction<ServiceLayerBizOutDtoAsync>());

            //VERIFY
            ex.Message.ShouldEqual("You cannot use an Async version of the DTO in a non-async action.");
        }
    }
}