using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BookMarket.Models;
namespace BookMarket.Data
{
    public partial class BookMarket_DBContext : DbContext
    {
        public DbSet<BookCarrello> BookCarrello { get; set; }
        public DbSet<BookLibri> BookLibri { get; set; }
        public DbSet<BookUtenti> BookUtenti { get; set; }
        public DbSet<EventsLog> EventsLog { get; set; }

        public BookMarket_DBContext(DbContextOptions<BookMarket_DBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Set default schema to dbo
            modelBuilder.HasDefaultSchema("dbo");

            //Set auto increment for GUID properties
            modelBuilder.Entity<BookCarrello>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
        }
    }
}
