// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal.Runners;
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
            var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
            var bizInstance = new BizActionValueInOut();
            var runner = new ActionServiceInOut<IBizActionValueInOut, int, string>(false, utData.WrappedConfig);

            //ATTEMPT
            var data = runner.RunBizActionDbAndInstance<string>(_dbContext, bizInstance, input);

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
            var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
            var bizInstance = new BizActionInOnly();
            var runner = new ActionServiceInOnly<IBizActionInOnly, BizDataIn>(false, utData.WrappedConfig);
            var input = new BizDataIn {Num = num};

            //ATTEMPT
            runner.RunBizActionDbAndInstance(_dbContext, bizInstance, input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
        }

        //-------------------------------------------------------
        //Sync, with mapping, no database access

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public void TestActionServiceInOutMappingOk(int num, bool hasErrors)
        {
            //SETUP
            var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
            utData.AddBizOutDtoMapping<ServiceLayerBizOutDto>();
            var bizInstance = new BizActionInOut();
            var runner = new ActionServiceInOut<IBizActionInOut, BizDataIn, BizDataOut>(false, utData.WrappedConfig);
            var inDto = new ServiceLayerBizInDto {Num = num};

            //ATTEMPT
            var data = runner.RunBizActionDbAndInstance<ServiceLayerBizOutDto>(_dbContext, bizInstance, inDto);

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
            var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
            utData.AddBizOutDtoMapping<ServiceLayerBizOutDto>();
            var bizInstance = new BizActionInOnly();
            var runner = new ActionServiceInOnly<IBizActionInOnly, BizDataIn>(false, utData.WrappedConfig);
            var inDto = new ServiceLayerBizInDto { Num = num };

            //ATTEMPT
            runner.RunBizActionDbAndInstance(_dbContext, bizInstance, inDto);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
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
                var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddBizOutDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOutWriteDb(context);
                var runner = new ActionServiceInOut<IBizActionInOutWriteDb, BizDataIn, BizDataOut>(true, utData.WrappedConfig);
                var inDto = new ServiceLayerBizInDto { Num = num };

                //ATTEMPT
                var data = runner.RunBizActionDbAndInstance<ServiceLayerBizOutDto>(context, bizInstance, inDto);

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
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
                utData.AddBizOutDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOnlyWriteDb(context);
                var runner = new ActionServiceInOnly<IBizActionInOnlyWriteDb, BizDataIn>(true, utData.WrappedConfig);
                var inDto = new ServiceLayerBizInDto {Num = num};

                //ATTEMPT
                runner.RunBizActionDbAndInstance(context, bizInstance, inDto);

                //VERIFY
                bizInstance.HasErrors.ShouldEqual(hasErrors);
                if (hasErrors)
                    context.LogEntries.Any().ShouldBeFalse();
                else
                    context.LogEntries.Single().LogText.ShouldEqual(num.ToString());
            }
        }

        //---------------------------------------------------------------
        //checking validation

        [Theory]
        [InlineData(123, false, false)]
        [InlineData(1, false, true)]
        [InlineData(1, true, false)]
        public void TestValidation(int num, bool hasErrors, bool doNotValidateSaveChanges)
        {
            //SETUP 
            var config = new GenericBizRunnerConfig { TurnOffCaching = true, DoNotValidateSaveChanges = doNotValidateSaveChanges };
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddBizOutDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOutWriteDb(context);
                var runner = new ActionServiceInOut<IBizActionInOutWriteDb, BizDataIn, BizDataOut>(true, utData.WrappedConfig);
                var inDto = new ServiceLayerBizInDto { Num = num };

                //ATTEMPT
                var data = runner.RunBizActionDbAndInstance<ServiceLayerBizOutDto>(context, bizInstance, inDto);

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

                var utData = NonDiSetup.SetupBizOutDtoMapping<ServiceLayerBizOutDto>(_noCachingConfig);
                var bizInstance = new BizActionOutOnlyWriteDb(context);
                var runner = new ActionServiceOutOnly<IBizActionOutOnlyWriteDb, BizDataOut>(true, utData.WrappedConfig);

                //ATTEMPT
                var data = runner.RunBizActionDbAndInstance<ServiceLayerBizOutDto>(context, bizInstance);

                //VERIFY
                bizInstance.HasErrors.ShouldBeFalse();
                context.LogEntries.Single().LogText.ShouldEqual("BizActionOutOnlyWriteDb");
            }
        }

        [Fact]
        public void TestActionServiceOutOnlyMappingOk()
        {
            //SETUP 
            var utData = NonDiSetup.SetupBizOutDtoMapping<ServiceLayerBizOutDto>(_noCachingConfig);
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionServiceOutOnly<IBizActionOutOnly, BizDataOut>(false, utData.WrappedConfig);

            //ATTEMPT
            var data = runner.RunBizActionDbAndInstance<ServiceLayerBizOutDto>(_dbContext, bizInstance);

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }

        [Fact]
        public void TestActionServiceOutOnlyNoDtoOk()
        {
            //SETUP 
            var utData = NonDiSetup.SetupBizInDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
            var bizInstance = new BizActionOutOnly();
            var runner = new ActionServiceOutOnly<IBizActionOutOnly, BizDataOut>(false, utData.WrappedConfig);

            //ATTEMPT
            var data = runner.RunBizActionDbAndInstance<BizDataOut>(_dbContext, bizInstance);

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }
    }
}