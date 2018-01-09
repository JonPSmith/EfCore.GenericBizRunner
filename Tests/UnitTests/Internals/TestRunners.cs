// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal.Runners;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.ActionsAsync;
using TestBizLayer.ActionsAsync.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Internals
{
    public class TestRunners
    {

        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };
        private readonly DbContext _dbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());

        //----------------------------------------------------
        //Sync, no mapping, no database access

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOutValuesOk(int input, bool hasErrors)
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto>(); //doesn't need a mapper, but mapper msutn't be null
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionServiceInOut<IBizActionValueInOut, int, string>(false, _noCachingConfig);

            //ATTEMPT
            var data = runner.RunBizActionDbAndInstance<string>(_dbContext, bizInstance, mapper, input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
            if (hasErrors)
                data.ShouldBeNull();
            else
                data.ShouldEqual(input.ToString());
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOnlyNoDtoOk(int num, bool hasErrors)
        {
            //SETUP 
            var mapper =
                SetupHelpers.CreateMapper<ServiceLayerBizInDto>(); //doesn't need a mapper, but mapper msutn't be null
            var bizInstance = new BizActionInOnly();
            var runner = new ActionServiceInOnly<IBizActionInOnly, BizDataIn>(false, _noCachingConfig);
            var input = new BizDataIn {Num = num};

            //ATTEMPT
            runner.RunBizActionDbAndInstance(_dbContext, bizInstance, mapper, input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
        }

        [Fact]
        public void TestActionServiceOutOnlyNoDtoOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto>(); //doesn't need a mapper, but mapper msutn't be null
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionServiceOutOnly<IBizActionOutOnly, BizDataOut>(false, _noCachingConfig);

            //ATTEMPT
            var data = runner.RunBizActionDbAndInstance<BizDataOut>(_dbContext, bizInstance, mapper);

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }

        //-------------------------------------------------------
        //Sync, with mapping, no database access

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOutMappingOk(int num, bool hasErrors)
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>(); 
            var bizInstance = new BizActionInOut();
            var runner = new ActionServiceInOut<IBizActionInOut, BizDataIn, BizDataOut>(false, _noCachingConfig);
            var inDto = new ServiceLayerBizInDto {Num = num};

            //ATTEMPT
            var data = runner.RunBizActionDbAndInstance<ServiceLayerBizOutDto>(_dbContext, bizInstance, mapper, inDto);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
            if (hasErrors)
                data.ShouldBeNull();
            else
                data.Output.ShouldEqual(inDto.Num.ToString());
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOnlyMappingOk(int num, bool hasErrors)
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();
            var bizInstance = new BizActionInOnly();
            var runner = new ActionServiceInOnly<IBizActionInOnly, BizDataIn>(false, _noCachingConfig);
            var inDto = new ServiceLayerBizInDto { Num = num };

            //ATTEMPT
            runner.RunBizActionDbAndInstance(_dbContext, bizInstance, mapper, inDto);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
        }

        [Fact]
        public void TestActionServiceOutOnlyMappingOk()
        {
            //SETUP 
            var mapper = SetupHelpers.CreateMapper<ServiceLayerBizOutDto>(); 
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionServiceOutOnly<IBizActionOutOnly, BizDataOut>(false, _noCachingConfig);

            //ATTEMPT
            var data = runner.RunBizActionDbAndInstance<ServiceLayerBizOutDto>(_dbContext, bizInstance, mapper);

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }


        //-------------------------------------------------------
        //Sync, with mapping, with database access

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOutMappingDatabaseOk(int num, bool hasErrors)
        {
            //SETUP 
            var config = new GenericBizRunnerConfig { TurnOffCaching = true, DoNotValidateSaveChanges = true };
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using(var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOutWriteDb(context);
                var runner = new ActionServiceInOut<IBizActionInOutWriteDb, BizDataIn, BizDataOut>(true, config);
                var inDto = new ServiceLayerBizInDto { Num = num };

                //ATTEMPT
                var data = runner.RunBizActionDbAndInstance<ServiceLayerBizOutDto>(context, bizInstance, mapper, inDto);

                //VERIFY
                bizInstance.HasErrors.ShouldEqual(hasErrors);
                if (hasErrors)
                    context.LogEntries.Any().ShouldBeFalse();
                else
                    context.LogEntries.Single().LogText.ShouldEqual(num.ToString());
            }
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOnlyMappingDatabaseOk(int num, bool hasErrors)
        {
            //SETUP 
            var config = new GenericBizRunnerConfig { TurnOffCaching = true, DoNotValidateSaveChanges = true };
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var mapper = SetupHelpers.CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOnlyWriteDb(context);
                var runner = new ActionServiceInOnly<IBizActionInOnlyWriteDb, BizDataIn>(true, config);
                var inDto = new ServiceLayerBizInDto {Num = num};

                //ATTEMPT
                runner.RunBizActionDbAndInstance(context, bizInstance, mapper, inDto);

                //VERIFY
                bizInstance.HasErrors.ShouldEqual(hasErrors);
                if (hasErrors)
                    context.LogEntries.Any().ShouldBeFalse();
                else
                    context.LogEntries.Single().LogText.ShouldEqual(num.ToString());
            }
        }

        [Fact]
        public void TestActionServiceOutOnlyMappingDatabaseOk()
        {
            //SETUP 
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var mapper = SetupHelpers.CreateMapper<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionOutOnlyWriteDb(context);
                var runner = new ActionServiceOutOnly<IBizActionOutOnlyWriteDb, BizDataOut>(true, _noCachingConfig);

                //ATTEMPT
                var data = runner.RunBizActionDbAndInstance<ServiceLayerBizOutDto>(context, bizInstance, mapper);

                //VERIFY
                bizInstance.HasErrors.ShouldBeFalse();
                context.LogEntries.Single().LogText.ShouldEqual("BizActionOutOnlyWriteDb");
            }
        }


    }
}