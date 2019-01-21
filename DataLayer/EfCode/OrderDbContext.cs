// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; } //#A
        public DbSet<Order> Orders { get; set; }

        public OrderDbContext(
            DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());
            modelBuilder.ApplyConfiguration(new OrderConfig());
            modelBuilder.ApplyConfiguration(new LineItemConfig());

            modelBuilder.Ignore<Review>(); 
            modelBuilder.Ignore<Author>();
            modelBuilder.Ignore<BookAuthor>(); 
        }
    }
}