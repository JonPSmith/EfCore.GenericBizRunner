// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using GenericBizRunner.Helpers;
using TestBizLayer.DbForTransactions;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Setup
{
    public class TestContext
    {
        [Fact]
        public void TestAddLogEntryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new LogEntry("Hello"));
                context.SaveChanges();

                //VERIFY
                context.LogEntries.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestAddLogEntryValidationOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new LogEntry("Hello"));
                var errors = context.SaveChangesWithValidation();

                //VERIFY
                context.LogEntries.Count().ShouldEqual(1);
                errors.Any().ShouldBeFalse();
            }
        }

        [Fact]
        public void TestAddLogEntryValidationError()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new LogEntry("!"));
                var errors = context.SaveChangesWithValidation();

                //VERIFY
                context.LogEntries.Count().ShouldEqual(0);
                errors.Any().ShouldBeTrue();
            }
        }
    }

}