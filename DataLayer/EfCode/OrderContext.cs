// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class OrderContext : DbContext
    {
        public DbSet<Book> Books { get; set; } //#A
        public DbSet<Order> Orders { get; set; }

        public OrderContext(
            DbContextOptions<OrderContext> options)
            : base(options)
        {
        }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());
            modelBuilder.ApplyConfiguration(new OrderConfig());
            modelBuilder.ApplyConfiguration(new LineItemConfig());

            modelBuilder.Ignore<Review>(); //#B
            modelBuilder.Ignore<PriceOffer>(); //#B
            modelBuilder.Ignore<Author>(); //#B
            modelBuilder.Ignore<BookAuthor>(); //#B
        }
    }
}