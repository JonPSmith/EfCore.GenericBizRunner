// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using GenericBizRunner.Configuration;
using GenericBizRunner.Internal.Runners;
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

namespace Tests.UnitTests.Internals
{
    public class TestRunnersAsync
    {
        private readonly IGenericBizRunnerConfig _noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };
        private readonly DbContext _dbContext = new TestDbContext(SqliteInMemory.CreateOptions<TestDbContext>());

        //-------------------------------------------------------
        //Async, no mapping, no database access

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOutValuesOkAsync(int num, bool hasErrors)
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
            var bizInstance = new BizActionInOutAsync();
            var runner = new ActionServiceInOutAsync<IBizActionInOutAsync, BizDataIn, BizDataOut>(false, utData.WrappedConfig);
            var input = new BizDataIn { Num = num };

            //ATTEMPT
            var data = await runner.RunBizActionDbAndInstanceAsync<BizDataOut>(_dbContext, bizInstance, input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
            if (hasErrors)
                data.ShouldBeNull();
            else
                data.Output.ShouldEqual(num.ToString());
        }

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOnlyNoDtoOk(int num, bool hasErrors)
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceInOnlyAsync<IBizActionInOnlyAsync, BizDataIn>(false, utData.WrappedConfig);
            var input = new BizDataIn { Num = num };

            //ATTEMPT
            await runner.RunBizActionDbAndInstanceAsync(_dbContext, bizInstance, input);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
        }

        //-------------------------------------------------------
        //Async, with mapping, no database access

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOutMappingOk(int num, bool hasErrors)
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
            utData.AddDtoMapping<ServiceLayerBizOutDto>();
            var bizInstance = new BizActionInOutAsync();
            var runner = new ActionServiceInOutAsync<IBizActionInOutAsync, BizDataIn, BizDataOut>(false, utData.WrappedConfig);
            var inDto = new ServiceLayerBizInDto { Num = num };

            //ATTEMPT
            var data = await runner.RunBizActionDbAndInstanceAsync<ServiceLayerBizOutDto>(_dbContext, bizInstance, inDto);

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
        public async Task TestActionServiceInOnlyMappingOk(int num, bool hasErrors)
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
            utData.AddDtoMapping<ServiceLayerBizOutDto>();
            var bizInstance = new BizActionInOnlyAsync();
            var runner = new ActionServiceInOnlyAsync<IBizActionInOnlyAsync, BizDataIn>(false, utData.WrappedConfig);
            var inDto = new ServiceLayerBizInDto { Num = num };

            //ATTEMPT
            await runner.RunBizActionDbAndInstanceAsync(_dbContext, bizInstance, inDto);

            //VERIFY
            bizInstance.HasErrors.ShouldEqual(hasErrors);
        }


        //-------------------------------------------------------
        //Async, with mapping, with database access

        [Theory]
        [InlineData(123, false)]
        [InlineData(-1, true)]
        public async Task TestActionServiceInOutMappingDatabaseOk(int num, bool hasErrors)
        {
            //SETUP 
            var config = new GenericBizRunnerConfig { TurnOffCaching = true, DoNotValidateSaveChanges = true };
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOutWriteDbAsync(context);
                var runner = new ActionServiceInOutAsync<IBizActionInOutWriteDbAsync, BizDataIn, BizDataOut>(true, utData.WrappedConfig);
                var inDto = new ServiceLayerBizInDto { Num = num };

                //ATTEMPT
                var data = await runner.RunBizActionDbAndInstanceAsync<ServiceLayerBizOutDto>(context, bizInstance, inDto);

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
        public async Task TestActionServiceInOnlyMappingDatabaseOk(int num, bool hasErrors)
        {
            //SETUP 
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(_noCachingConfig);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOnlyWriteDbAsync(context);
                var runner = new ActionServiceInOnlyAsync<IBizActionInOnlyWriteDbAsync, BizDataIn>(true, utData.WrappedConfig);
                var inDto = new ServiceLayerBizInDto { Num = num };

                //ATTEMPT
                await runner.RunBizActionDbAndInstanceAsync(context, bizInstance, inDto);

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
        public async Task TestValidation(int num, bool hasErrors, bool doNotValidateSaveChanges)
        {
            //SETUP 
            var config = new GenericBizRunnerConfig { TurnOffCaching = true, DoNotValidateSaveChanges = doNotValidateSaveChanges };
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOutWriteDbAsync(context);
                var runner = new ActionServiceInOutAsync<IBizActionInOutWriteDbAsync, BizDataIn, BizDataOut>(true, utData.WrappedConfig);
                var inDto = new ServiceLayerBizInDto { Num = num };

                //ATTEMPT
                var data = await runner.RunBizActionDbAndInstanceAsync<ServiceLayerBizOutDto>(context, bizInstance, inDto);

                //VERIFY
                bizInstance.HasErrors.ShouldEqual(hasErrors);
                if (hasErrors)
                    context.LogEntries.Any().ShouldBeFalse();
                else
                    context.LogEntries.Single().LogText.ShouldEqual(num.ToString());
            }
        }

        [Fact]
        public async Task TestActionServiceOutOnlyMappingDatabaseOk()
        {
            //SETUP 
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizOutDto>(_noCachingConfig);
                var bizInstance = new BizActionOutOnlyWriteDbAsync(context);
                var runner = new ActionServiceOutOnlyAsync<IBizActionOutOnlyWriteDbAsync, BizDataOut>(true, utData.WrappedConfig);

                //ATTEMPT
                var data = await runner.RunBizActionDbAndInstanceAsync<ServiceLayerBizOutDto>(context, bizInstance);

                //VERIFY
                bizInstance.HasErrors.ShouldBeFalse();
                context.LogEntries.Single().LogText.ShouldEqual("BizActionOutOnlyWriteDbAsync");
            }
        }

        [Fact]
        public async Task TestActionServiceOutOnlyMappingOk()
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizOutDto>(_noCachingConfig);
            var bizInstance = new BizActionOutOnlyAsync();
            var runner = new ActionServiceOutOnlyAsync<IBizActionOutOnlyAsync, BizDataOut>(false, utData.WrappedConfig);

            //ATTEMPT
            var data = await runner.RunBizActionDbAndInstanceAsync<ServiceLayerBizOutDto>(_dbContext, bizInstance);

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }

        [Fact]
        public async Task TestActionServiceOutOnlyNoDtoOk()
        {
            //SETUP 
            var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizOutDto>(_noCachingConfig);
            var bizInstance = new BizActionOutOnlyAsync();
            var runner = new ActionServiceOutOnlyAsync<IBizActionOutOnlyAsync, BizDataOut>(false, utData.WrappedConfig);

            //ATTEMPT
            var data = await runner.RunBizActionDbAndInstanceAsync<BizDataOut>(_dbContext, bizInstance);

            //VERIFY
            bizInstance.HasErrors.ShouldBeFalse();
            data.Output.ShouldEqual("Result");
        }
    }
}