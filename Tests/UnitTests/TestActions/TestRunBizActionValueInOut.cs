using System;
using AutoMapper;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal.Runners;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestActions
{
    public class TestRunBizActionValueInOut
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };
        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
        //Beacause this is ValueInOut then there is no need for a mapper, but the ActionService checks that the Mapper isn't null
        private readonly IMapper _emptyMapper = new Mapper(new MapperConfiguration(cfg => {}));

        [Fact]
        public void TestCallInternalInOutServiceOk()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionServiceInOut<IBizActionValueInOut, int, string>(false, _noCachingConfig);
            var input = 123;

            //ATTEMPT
            var data = runner.RunBizActionDbAndInstance<string>(_emptyDbContext, bizInstance, _emptyMapper, input);

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.ShouldEqual("123");
            bizInstance.Message.ShouldEqual("All Ok");
        }

        [Fact]
        public void TestRunBizActionValueInOutDirectOk()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _emptyMapper, _noCachingConfig);
            var input = 123;

            //ATTEMPT
            var data = runner.RunBizAction<string>(input);

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.ShouldEqual("123");
            bizInstance.Message.ShouldEqual("All Ok");
        }

        [Fact]
        public void TestRunBizActionValueInOutDirectBizErrorOk()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _emptyMapper, _noCachingConfig);
            var input = -1;

            //ATTEMPT
            var data = runner.RunBizAction<string>(input);

            //VERIFY
            bizInstance.HasErrors.ShouldBeTrue();
            data.ShouldBeNull();
            bizInstance.Message.ShouldEqual("Failed with 1 error");
        }

        //---------------------------------------------------------------
        //error checking

        [Fact]
        public void TestInputIsBad()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _emptyMapper, _noCachingConfig);
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>( () => runner.RunBizAction<string>(input));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = String, to type Int32. Expected a DTO of type GenericActionToBizDto<Int32,String>");
        }

        [Fact]
        public void TestCallHasNoOutputBad()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _emptyMapper, _noCachingConfig);
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionValueInOut needed 'In' but the Business class had a different setup of 'InOut'");
        }

        [Fact]
        public void TestCallHasNoInputBad()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _emptyMapper, _noCachingConfig);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction<string>());

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionValueInOut needed 'Out' but the Business class had a different setup of 'InOut'");
        }

        [Fact]
        public void TestUseAsyncBizInWithSyncBad()
        {
            //SETUP 
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionService<IBizActionValueInOut>(_emptyDbContext, bizInstance, _emptyMapper, _noCachingConfig);
            var input = new ServiceLayerBizInDtoAsync();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction<string>(input));

            //VERIFY
            ex.Message.ShouldEqual("You cannot use an Async version of the DTO in a non-async action.");
        }
    }
}