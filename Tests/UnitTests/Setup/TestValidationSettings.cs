// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.Actions;
using TestBizLayer.Actions.Concrete;
using TestBizLayer.ActionsAsync;
using TestBizLayer.ActionsAsync.Concrete;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;
using Tests.DTOs;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Setup
{
    public class TestValidationSettings
    {
        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void TestNormalValidateActionsBasedOnConfig(bool doNotValidate, bool hasErrors)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var config = new GenericBizRunnerConfig { TurnOffCaching = true, DoNotValidateSaveChanges = doNotValidate };
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOnlyWriteDb(context);
                var runner = new ActionService<IBizActionInOnlyWriteDb>(context, bizInstance, utData.WrappedConfig);
                var input = new BizDataIn { Num = 1 };

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
                    context.LogEntries.Single().LogText.ShouldEqual("1");
                }
            }
        }

        [Fact]
        public void ForceValidateOff()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var config = new GenericBizRunnerConfig { TurnOffCaching = true};
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOnlyWriteDbForceValidateOff(context);
                var runner = new ActionService<IBizActionInOnlyWriteDbForceValidateOff>(context, bizInstance, utData.WrappedConfig);
                var input = new BizDataIn {Num = 1};

                //ATTEMPT
                runner.RunBizAction(input);

                //VERIFY
                runner.Status.HasErrors.ShouldBeFalse();
                context.LogEntries.Single().LogText.ShouldEqual("1");
            }
        }

        [Fact]
        public void ForceValidateOn()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var config = new GenericBizRunnerConfig { TurnOffCaching = true, DoNotValidateSaveChanges = true};
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionInOnlyWriteDbForceValidateOn(context);
                var runner =
                    new ActionService<IBizActionInOnlyWriteDbForceValidateOn>(context, bizInstance, utData.WrappedConfig);
                var input = new BizDataIn { Num = 1 };

                //ATTEMPT
                runner.RunBizAction(input);

                //VERIFY
                runner.Status.HasErrors.ShouldBeTrue();
                context.LogEntries.Any().ShouldBeFalse();
            }
        }

        [Fact]
        public void TestSqlErrorHandlerNotInPlace()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity { UniqueString = "Hello" });
                context.SaveChanges();

                var config = new GenericBizRunnerConfig { TurnOffCaching = true};
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionCheckSqlErrorHandlerWriteDb(context);
                var runner = new ActionService<IBizActionCheckSqlErrorHandlerWriteDb>(context, bizInstance, utData.WrappedConfig);

                //ATTEMPT
                var ex = Assert.Throws<DbUpdateException>(() => runner.RunBizAction("Hello"));

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'UNIQUE constraint failed: UniqueEntities.UniqueString'.");
            }
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(19, false)]
        public void TestSqlErrorHandlerWorksOk(int sqlErrorCode, bool shouldThrowException)
        {
            IStatusGeneric CatchUniqueError(Exception e, DbContext context)
            {
                var dbUpdateEx = e as DbUpdateException;
                var sqliteError = dbUpdateEx?.InnerException as SqliteException;
                return sqliteError?.SqliteErrorCode == sqlErrorCode
                    ? new StatusGenericHandler().AddError("Unique constraint failed")
                    : null;
            }

            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity { UniqueString = "Hello" });
                context.SaveChanges();

                var config = new GenericBizRunnerConfig
                {
                    TurnOffCaching = true,
                    SaveChangesExceptionHandler = CatchUniqueError
                };
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionCheckSqlErrorHandlerWriteDb(context);
                var runner = new ActionService<IBizActionCheckSqlErrorHandlerWriteDb>(context, bizInstance, utData.WrappedConfig);

                //ATTEMPT
                try
                {
                    runner.RunBizAction("Hello");
                }
                //VERIFY
                catch (Exception e)
                {
                    shouldThrowException.ShouldBeTrue();
                    return;
                }

                shouldThrowException.ShouldBeFalse();
                runner.Status.GetAllErrors().ShouldEqual("Unique constraint failed");
            }
        }

        [Fact]
        public void TestSqlErrorHandlerWorksEvenIfValidationIsTurnedOff()
        {
            IStatusGeneric CatchUniqueError(Exception e, DbContext context)
            {
                var dbUpdateEx = e as DbUpdateException;
                var sqliteError = dbUpdateEx?.InnerException as SqliteException;
                return sqliteError?.SqliteErrorCode == 19
                    ? new StatusGenericHandler().AddError("Unique constraint failed")
                    : null;
            }

            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity { UniqueString = "Hello" });
                context.SaveChanges();

                var config = new GenericBizRunnerConfig
                {
                    TurnOffCaching = true,
                    DoNotValidateSaveChanges = true,
                    SaveChangesExceptionHandler = CatchUniqueError
                };
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionCheckSqlErrorHandlerWriteDb(context);
                var runner = new ActionService<IBizActionCheckSqlErrorHandlerWriteDb>(context, bizInstance, utData.WrappedConfig);

                //ATTEMPT
                runner.RunBizAction("Hello");

                //VERIFY
                runner.Status.GetAllErrors().ShouldEqual("Unique constraint failed");
            }
        }


        [Theory]
        [InlineData(1, true)]
        [InlineData(19, false)]
        public async Task TestSqlErrorHandlerWorksOkAsync(int sqlErrorCode, bool shouldThrowException)
        {
            IStatusGeneric CatchUniqueError(Exception e, DbContext context)
            {
                var dbUpdateEx = e as DbUpdateException;
                var sqliteError = dbUpdateEx?.InnerException as SqliteException;
                return sqliteError?.SqliteErrorCode == sqlErrorCode
                    ? new StatusGenericHandler().AddError("Unique constraint failed")
                    : null;
            }

            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity { UniqueString = "Hello" });
                context.SaveChanges();

                var config = new GenericBizRunnerConfig
                {
                    TurnOffCaching = true,
                    SaveChangesExceptionHandler = CatchUniqueError
                };
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionCheckSqlErrorHandlerWriteDbAsync(context);
                var runner = new ActionServiceAsync<IBizActionCheckSqlErrorHandlerWriteDbAsync>(context, bizInstance, utData.WrappedConfig);

                //ATTEMPT
                try
                {
                    await runner.RunBizActionAsync("Hello");
                }
                //VERIFY
                catch (Exception e)
                {
                    shouldThrowException.ShouldBeTrue();
                    return;
                }

                shouldThrowException.ShouldBeFalse();
                runner.Status.GetAllErrors().ShouldEqual("Unique constraint failed");
            }
        }

        [Fact]
        public async Task TestSqlErrorHandlerWorksEvenIfValidationIsTurnedOffAsync()
        {
            IStatusGeneric CatchUniqueError(Exception e, DbContext context)
            {
                var dbUpdateEx = e as DbUpdateException;
                var sqliteError = dbUpdateEx?.InnerException as SqliteException;
                return sqliteError?.SqliteErrorCode == 19
                    ? new StatusGenericHandler().AddError("Unique constraint failed")
                    : null;
            }

            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity { UniqueString = "Hello" });
                context.SaveChanges();

                var config = new GenericBizRunnerConfig
                {
                    TurnOffCaching = true,
                    DoNotValidateSaveChanges = true,
                    SaveChangesExceptionHandler = CatchUniqueError
                };
                var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
                utData.AddDtoMapping<ServiceLayerBizOutDto>();
                var bizInstance = new BizActionCheckSqlErrorHandlerWriteDbAsync(context);
                var runner = new ActionServiceAsync<IBizActionCheckSqlErrorHandlerWriteDbAsync>(context, bizInstance, utData.WrappedConfig);

                //ATTEMPT
                await runner.RunBizActionAsync("Hello");

                //VERIFY
                runner.Status.GetAllErrors().ShouldEqual("Unique constraint failed");
            }
        }
    }
}