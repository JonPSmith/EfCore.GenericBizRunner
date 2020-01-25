// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using GenericBizRunner;
using GenericBizRunner.Configuration;
using GenericBizRunner.Helpers;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using TestBizLayer.DbForTransactions;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Setup
{
    public class TestBeforeSaveChangesAsync
    {
        [Fact]
        public async Task TestNoBeforeSaveChangesMethodProvided()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new UniqueEntity {UniqueString = "bad word"});
                var status = await context.SaveChangesWithValidationAsync();

                //VERIFY
                status.HasErrors.ShouldBeFalse(status.GetAllErrors());
            }

            using (var context = new TestDbContext(options))
            {
                context.UniqueEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestBeforeSaveChangesMethodProvidedNoError()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var config = new GenericBizRunnerConfig()
                {
                    BeforeSaveChanges = FailOnBadWord
                };

                //ATTEMPT
                context.Add(new UniqueEntity {UniqueString = "good word"});
                var status = await context.SaveChangesWithValidationAsync(config);

                //VERIFY
                status.HasErrors.ShouldBeFalse(status.GetAllErrors());
            }

            using (var context = new TestDbContext(options))
            {
                context.UniqueEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestBeforeSaveChangesMethodProvidedWithError()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var config = new GenericBizRunnerConfig()
                {
                    BeforeSaveChanges = FailOnBadWord
                };

                //ATTEMPT
                context.Add(new UniqueEntity {UniqueString = "bad word"});
                var status = await context.SaveChangesWithValidationAsync(config);

                //VERIFY
                status.HasErrors.ShouldBeTrue();
                status.GetAllErrors().ShouldEqual("The UniqueEntity class contained a bad word.");
            }

            using (var context = new TestDbContext(options))
            {
                context.UniqueEntities.Count().ShouldEqual(0);
            }
        }

        //-------------------------------------------------
        //BeforeSaveChanges test setup

        private IStatusGeneric FailOnBadWord(DbContext context)
        {
            var status = new StatusGenericHandler();
            var entriesToCheck = context.ChangeTracker.Entries()
                .Where(e =>
                    (e.State == EntityState.Added) ||
                    (e.State == EntityState.Modified));
            foreach (var entity in entriesToCheck)
            {
                if (entity.Entity is UniqueEntity normalInstance && normalInstance.UniqueString.Contains("bad"))
                    status.AddError($"The {nameof(UniqueEntity)} class contained a bad word.");
            }

            return status;
        }
    }
}