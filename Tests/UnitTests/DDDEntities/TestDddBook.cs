// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;
using Tests.Helpers;

namespace Tests.UnitTests.DDDEntities
{
    public class TestDddBook
    {
        [Fact]
        public void TestAddReviewToBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.Include(x => x.Reviews).First();
                book.AddReview(5, "comment", "user");
                context.SaveChanges();

                //VERIFY
                book.Reviews.Count().ShouldEqual(1);
                context.Set<Review>().Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void TestRemoveReviewBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.Include(x => x.Reviews).Single(x => x.Reviews.Count() == 2);
                book.RemoveReview(book.Reviews.LastOrDefault());
                context.SaveChanges();

                //VERIFY
                book.Reviews.Count().ShouldEqual(1);
                context.Set<Review>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestAddPromotionBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.First();
                book.AddPromotion(book.OrgPrice / 2, "Half price today");
                context.SaveChanges();

                //VERIFY
                book.ActualPrice.ShouldEqual(book.OrgPrice / 2);
            }
        }

        [Fact]
        public void TestRemovePromotionBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.OrderByDescending(x => x.BookId).First();
                book.RemovePromotion();
                context.SaveChanges();

                //VERIFY
                book.ActualPrice.ShouldEqual(book.OrgPrice);
            }
        }

    }

}