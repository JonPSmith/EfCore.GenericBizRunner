// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace TestBizLayer.DbForTransactions
{
    public class TestDbContext : DbContext
    {
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<UniqueEntity> UniqueEntities { get; set; }

        public TestDbContext(
            DbContextOptions<TestDbContext> options)
            : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UniqueEntity>().HasIndex(p => p.UniqueString).IsUnique().HasName("UniqueError_UniqueEntity_UniqueString");
        }


    }
}