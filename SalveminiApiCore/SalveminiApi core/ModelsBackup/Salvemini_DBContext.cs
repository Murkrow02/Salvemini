using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SalveminiApi_core.Models
{
    public partial class Salvemini_DBContext : DbContext
    {
        public Salvemini_DBContext()
        {
        }

        public Salvemini_DBContext(DbContextOptions<Salvemini_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ads> Ads { get; set; }
        public virtual DbSet<Analytics> Analytics { get; set; }
        public virtual DbSet<AppInfo> AppInfo { get; set; }
        public virtual DbSet<Avvisi> Avvisi { get; set; }
        public virtual DbSet<BookCarrello> BookCarrello { get; set; }
        public virtual DbSet<BookLibri> BookLibri { get; set; }
        public virtual DbSet<BookUtenti> BookUtenti { get; set; }
        public virtual DbSet<CoinGuadagnate> CoinGuadagnate { get; set; }
        public virtual DbSet<CoinSpese> CoinSpese { get; set; }
        public virtual DbSet<Commenti> Commenti { get; set; }
        public virtual DbSet<Coupon> Coupon { get; set; }
        public virtual DbSet<CouponAttivi> CouponAttivi { get; set; }
        public virtual DbSet<Domande> Domande { get; set; }
        public virtual DbSet<EventsLog> EventsLog { get; set; }
        public virtual DbSet<FlappyClassifica> FlappyClassifica { get; set; }
        public virtual DbSet<FlappyMonete> FlappyMonete { get; set; }
        public virtual DbSet<FlappySkin> FlappySkin { get; set; }
        public virtual DbSet<Giornalino> Giornalino { get; set; }
        public virtual DbSet<Materie> Materie { get; set; }
        public virtual DbSet<Notifiche> Notifiche { get; set; }
        public virtual DbSet<OggettiSondaggi> OggettiSondaggi { get; set; }
        public virtual DbSet<SalveminiCard> SalveminiCard { get; set; }
        public virtual DbSet<Sondaggi> Sondaggi { get; set; }
        public virtual DbSet<Utenti> Utenti { get; set; }
        public virtual DbSet<VotiSondaggi> VotiSondaggi { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=68.168.220.50;Initial Catalog=Salvemini_DB;Persist Security Info=True;User ID=Salvemini_DB;Password=Ctpb09~1");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ads>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Immagine).HasMaxLength(2000);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Url).HasMaxLength(2000);
            });

            modelBuilder.Entity<Analytics>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<AppInfo>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.AppVersion)
                    .HasColumnType("decimal(10, 1)")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.OrariVersion).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Avvisi>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Creazione).HasColumnType("datetime");

                entity.Property(e => e.Descrizione).HasMaxLength(3000);

                entity.Property(e => e.IdCreatore).HasColumnName("idCreatore");

                entity.Property(e => e.Immagini).HasMaxLength(3000);

                entity.Property(e => e.Titolo)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.IdCreatoreNavigation)
                    .WithMany(p => p.Avvisi)
                    .HasForeignKey(d => d.IdCreatore)
                    .HasConstraintName("FK_Avvisi_Utenti");
            });

            modelBuilder.Entity<BookCarrello>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdLibro).HasColumnName("idLibro");

                entity.Property(e => e.IdUtente).HasColumnName("idUtente");

                entity.HasOne(d => d.IdLibroNavigation)
                    .WithMany(p => p.BookCarrello)
                    .HasForeignKey(d => d.IdLibro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookCarrello_BookLibri");

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithMany(p => p.BookCarrello)
                    .HasForeignKey(d => d.IdUtente)
                    .HasConstraintName("FK_BookCarrello_BookUtenti");
            });

            modelBuilder.Entity<BookLibri>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Codice)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.DataCaricamento).HasColumnType("datetime");

                entity.Property(e => e.IdUtente).HasColumnName("idUtente");

                entity.Property(e => e.Materia)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Prezzo).HasColumnType("money");

                entity.HasOne(d => d.CompratoDaNavigation)
                    .WithMany(p => p.BookLibriCompratoDaNavigation)
                    .HasForeignKey(d => d.CompratoDa)
                    .HasConstraintName("FK_BookLibri_BookUtenti1");

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithMany(p => p.BookLibriIdUtenteNavigation)
                    .HasForeignKey(d => d.IdUtente)
                    .HasConstraintName("FK_BookLibri_BookUtenti");
            });

            modelBuilder.Entity<BookUtenti>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppuntamentoFinale).HasColumnType("datetime");

                entity.Property(e => e.AppuntamentoRilascio).HasColumnType("datetime");

                entity.Property(e => e.AppuntamentoRitiro).HasColumnType("datetime");

                entity.Property(e => e.Cognome)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Ip).HasMaxLength(50);

                entity.Property(e => e.LastMailSent).HasColumnType("datetime");

                entity.Property(e => e.Mail).HasMaxLength(150);

                entity.Property(e => e.MailToken).HasMaxLength(50);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Token).HasMaxLength(50);
            });

            modelBuilder.Entity<CoinGuadagnate>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attivazione).HasColumnType("datetime");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.IdUtente).HasColumnName("idUtente");

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithMany(p => p.CoinGuadagnate)
                    .HasForeignKey(d => d.IdUtente)
                    .HasConstraintName("FK_CoinGuadagnate_Utenti");
            });

            modelBuilder.Entity<CoinSpese>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attivazione).HasColumnType("datetime");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.IdOggetto).HasColumnName("idOggetto");

                entity.Property(e => e.IdUtente).HasColumnName("idUtente");

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithMany(p => p.CoinSpese)
                    .HasForeignKey(d => d.IdUtente)
                    .HasConstraintName("FK_CoinSpese_Utenti");
            });

            modelBuilder.Entity<Commenti>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Commento)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Creazione).HasColumnType("datetime");

                entity.Property(e => e.IdPost).HasColumnName("idPost");

                entity.Property(e => e.IdUtente).HasColumnName("idUtente");

                entity.HasOne(d => d.IdPostNavigation)
                    .WithMany(p => p.Commenti)
                    .HasForeignKey(d => d.IdPost)
                    .HasConstraintName("FK_Commenti_Domande");

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithMany(p => p.Commenti)
                    .HasForeignKey(d => d.IdUtente)
                    .HasConstraintName("FK_Commenti_Utenti");
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.Codice)
                    .HasName("PK_Coupons");

                entity.Property(e => e.Codice).ValueGeneratedNever();

                entity.Property(e => e.Attivo)
                    .IsRequired()
                    .HasDefaultValueSql("('1')");

                entity.Property(e => e.Creazione).HasColumnType("datetime");

                entity.Property(e => e.IdCreatore).HasColumnName("idCreatore");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Raggio).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.XAttivazione)
                    .HasColumnName("xAttivazione")
                    .HasColumnType("decimal(18, 8)");

                entity.Property(e => e.YAttivazione)
                    .HasColumnName("yAttivazione")
                    .HasColumnType("decimal(18, 8)");

                entity.HasOne(d => d.IdCreatoreNavigation)
                    .WithMany(p => p.Coupon)
                    .HasForeignKey(d => d.IdCreatore)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Coupon_Utenti");
            });

            modelBuilder.Entity<CouponAttivi>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdCoupon).HasColumnName("idCoupon");

                entity.Property(e => e.IdUtente).HasColumnName("idUtente");

                entity.HasOne(d => d.IdCouponNavigation)
                    .WithMany(p => p.CouponAttivi)
                    .HasForeignKey(d => d.IdCoupon)
                    .HasConstraintName("FK_CouponAttivi_Coupon");

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithMany(p => p.CouponAttivi)
                    .HasForeignKey(d => d.IdUtente)
                    .HasConstraintName("FK_CouponAttivi_Utenti");
            });

            modelBuilder.Entity<Domande>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Creazione).HasColumnType("datetime");

                entity.Property(e => e.Domanda)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.IdUtente).HasColumnName("idUtente");
            });

            modelBuilder.Entity<EventsLog>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Data).HasColumnType("datetime");

                entity.Property(e => e.Evento)
                    .IsRequired()
                    .HasMaxLength(3000);
            });

            modelBuilder.Entity<FlappyClassifica>(entity =>
            {
                entity.HasKey(e => e.IdUtente);

                entity.Property(e => e.IdUtente)
                    .HasColumnName("idUtente")
                    .ValueGeneratedNever();

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithOne(p => p.FlappyClassifica)
                    .HasForeignKey<FlappyClassifica>(d => d.IdUtente)
                    .HasConstraintName("FK_FlappyClassifica_Utenti");
            });

            modelBuilder.Entity<FlappyMonete>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<FlappySkin>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Immagini)
                    .IsRequired()
                    .HasMaxLength(550);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<Giornalino>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Data).HasColumnType("datetime");
            });

            modelBuilder.Entity<Materie>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DesMateria)
                    .IsRequired()
                    .HasColumnName("desMateria")
                    .HasMaxLength(500);

                entity.Property(e => e.Materia).HasMaxLength(200);
            });

            modelBuilder.Entity<Notifiche>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Creazione).HasColumnType("datetime");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.IdPost).HasColumnName("idPost");

                entity.Property(e => e.IdUtente).HasColumnName("idUtente");

                entity.HasOne(d => d.IdUtenteNavigation)
                    .WithMany(p => p.Notifiche)
                    .HasForeignKey(d => d.IdUtente)
                    .HasConstraintName("FK_Notifiche_Utenti");
            });

            modelBuilder.Entity<OggettiSondaggi>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdSondaggio).HasColumnName("idSondaggio");

                entity.Property(e => e.Immagine).HasMaxLength(300);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.IdSondaggioNavigation)
                    .WithMany(p => p.OggettiSondaggi)
                    .HasForeignKey(d => d.IdSondaggio)
                    .HasConstraintName("FK_OggettiSondaggi_Sondaggi");
            });

            modelBuilder.Entity<SalveminiCard>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descrizione)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Immagine)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Sondaggi>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attivo)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.Creazione).HasColumnType("datetime");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Utenti)
                 .WithMany(p => p.Sondaggi)
                 .HasForeignKey(d => d.Creatore)
                 .OnDelete(DeleteBehavior.NoAction)
                 .HasConstraintName("FK_Sondaggi_Utenti");
            });

            modelBuilder.Entity<Utenti>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ArgoToken)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Cognome)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Corso)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Creazione).HasColumnType("datetime");

                entity.Property(e => e.Immagine)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.LastAdWatched).HasColumnType("datetime");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.SCoin)
                    .HasColumnName("sCoin")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.Sesso)
                    .IsRequired()
                    .HasMaxLength(1);
            });

            modelBuilder.Entity<VotiSondaggi>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdSondaggio).HasColumnName("idSondaggio");

                entity.HasOne(d => d.IdSondaggioNavigation)
                    .WithMany(p => p.VotiSondaggi)
                    .HasForeignKey(d => d.IdSondaggio)
                    .HasConstraintName("FK_VotiSondaggi_Sondaggi");

                entity.HasOne(d => d.UtenteNavigation)
                    .WithMany(p => p.VotiSondaggi)
                    .HasForeignKey(d => d.Utente)
                    .HasConstraintName("FK_VotiSondaggi_Utenti");

                entity.HasOne(d => d.VotoNavigation)
                    .WithMany(p => p.VotiSondaggi)
                    .HasForeignKey(d => d.Voto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VotiSondaggi_OggettiSondaggi");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
