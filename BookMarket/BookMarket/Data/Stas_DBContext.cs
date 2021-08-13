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

            modelBuilder.Entity<BookLibri>()
               .HasOne(x => x.Proprietario)
               .WithMany(m => m.LibriRegistrati)
               .HasForeignKey(x => x.IdProprietario);

            modelBuilder.Entity<BookLibri>()
              .HasOne(x => x.Acquirente)
              .WithMany(m => m.LibriVenduti)
              .HasForeignKey(x => x.IdAcquirente);

            modelBuilder.Entity<BookCarrello>().HasOne(o => o.Utente).WithMany(m => m.BookCarrello).OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<BookCarrello>().HasOne(o => o.Libro).WithMany(m => m.BookCarrello).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
