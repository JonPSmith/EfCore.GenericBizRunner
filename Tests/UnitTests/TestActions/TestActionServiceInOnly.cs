// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

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
    public class TestActionServiceInOnly
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };

        //This action does not access the database, but the ActionService checks that the dbContext isn't null
        private readonly DbContext _emptyDbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());
        readonly IMapper _mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOutDirectOk(int num, bool hasErrors)
        {
            //SETUP         
            var bizInstance = new BizActionInOnly();
            var runner = new ActionService<IBizActionInOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = new BizDataIn { Num = num};

            //ATTEMPT
            runner.RunBizAction(input);

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
        public void TestActionServiceInOutDtosOk(int num, bool hasErrors)
        {
            //SETUP         
            var bizInstance = new BizActionInOnly();
            var runner = new ActionService<IBizActionInOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = new ServiceLayerBizInDto { Num = num };

            //ATTEMPT
            runner.RunBizAction(input);

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
        public void TestActionServiceErrorInSetupOk()
        {
            //SETUP         
            var bizInstance = new BizActionInOnly();
            var runner = new ActionService<IBizActionInOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = runner.GetDto<ServiceLayerBizInDto>(x => { x.RaiseErrorInSetupSecondaryData = true; });

            //ATTEMPT
            runner.RunBizAction(input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(true);
            bizInstance.Errors.Single().ErrorMessage.ShouldEqual("Error in SetupSecondaryData");
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOutDtosDatabaseOk(int num, bool hasErrors)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var bizInstance = new BizActionInOnlyWriteDb(context);
                var runner =
                    new ActionService<IBizActionInOnlyWriteDb>(context, bizInstance, _mapper, _noCachingConfig);
                var input = new ServiceLayerBizInDto {Num = num};

                //ATTEMPT
                runner.RunBizAction(input);

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
        public void TestCallHasOutputBad()
        {
            //SETUP 
            var bizInstance = new BizActionInOnly();
            var runner = new ActionService<IBizActionInOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction<string>(input));

            //VERIFY
            ex.Message.ShouldEqual("Your call of IBizActionInOnly needed 'InOut' but the Business class had a different setup of 'In'");
        }

        //---------------------------------------------------------------
        //error checking

        [Fact]
        public void TestInputIsBad()
        {
            //SETUP 
            var bizInstance = new BizActionInOnly();
            var runner = new ActionService<IBizActionInOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = "string";

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>( () => runner.RunBizAction(input));

            //VERIFY
            ex.Message.ShouldEqual("Indirect copy to biz action. from type = String, to type BizDataIn. Expected a DTO of type GenericActionToBizDto<BizDataIn,String>");
        }


        [Fact]
        public void TestUseAsyncBizInWithSyncBad()
        {
            //SETUP 
            var bizInstance = new BizActionInOnly();
            var runner = new ActionService<IBizActionInOnly>(_emptyDbContext, bizInstance, _mapper, _noCachingConfig);
            var input = new ServiceLayerBizInDtoAsync();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => runner.RunBizAction(input));

            //VERIFY
            ex.Message.ShouldEqual("You cannot use an Async version of the DTO in a non-async action.");
        }
    }
}