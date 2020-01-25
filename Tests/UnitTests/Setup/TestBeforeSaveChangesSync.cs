// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
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
    public class TestBeforeSaveChangesSync
    {
        [Fact]
        public void TestNoBeforeSaveChangesMethodProvided()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add( new UniqueEntity {UniqueString = "bad word"});
                var status = context.SaveChangesWithValidation();

                //VERIFY
                status.HasErrors.ShouldBeFalse(status.GetAllErrors());
            }
            using (var context = new TestDbContext(options))
            {
                context.UniqueEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestBeforeSaveChangesMethodProvidedNoError()
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
                context.Add(new UniqueEntity { UniqueString = "good word" });
                var status = context.SaveChangesWithValidation(config);

                //VERIFY
                status.HasErrors.ShouldBeFalse(status.GetAllErrors());
            }
            using (var context = new TestDbContext(options))
            {
                context.UniqueEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestBeforeSaveChangesMethodProvidedWithError()
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
                context.Add(new UniqueEntity { UniqueString = "bad word" });
                var status = context.SaveChangesWithValidation(config);

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