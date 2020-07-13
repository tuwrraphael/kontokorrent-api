using Microsoft.EntityFrameworkCore;

namespace Kontokorrent.Impl.EF
{
    public class KontokorrentContext : DbContext
    {
        public KontokorrentContext(DbContextOptions<KontokorrentContext> contextOptions) : base(contextOptions)
        {

        }
        public DbSet<Kontokorrent> Kontokorrent { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Bezahlung> Bezahlung { get; set; }
        public DbSet<BenutzerSecret> BenutzerSecret { get; set; }
        public DbSet<EmfpaengerInBezahlung> EmfpaengerInBezahlung { get; set; }
        public DbSet<BenutzerKontokorrent> BenutzerKontokorrent { get; set; }
        public DbSet<EinladungsCode> EinladungsCode { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kontokorrent>()
                .HasMany(p => p.Personen)
                .WithOne(p => p.Kontokorrent)
                .HasForeignKey(p => p.KontokorrentId);
            modelBuilder.Entity<Kontokorrent>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Kontokorrent>()
                .Property(p => p.Name);
            modelBuilder.Entity<Kontokorrent>()
                .Property(p => p.OeffentlicherName)
                .IsRequired();
            modelBuilder.Entity<Kontokorrent>().
                HasAlternateKey(p => p.OeffentlicherName);
            modelBuilder.Entity<Kontokorrent>().
                Property(p => p.OeffentlicherName)
                .HasColumnName("Secret");

            modelBuilder.Entity<Person>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Person>().
                Property(p => p.Name)
                .IsRequired();
            modelBuilder.Entity<Person>().
                HasAlternateKey(p => new { p.KontokorrentId, p.Name });

            modelBuilder.Entity<Bezahlung>().
                HasKey(p => p.Id);
            modelBuilder.Entity<Bezahlung>().
                Property(p => p.KontokorrentId)
                .IsRequired();
            modelBuilder.Entity<Bezahlung>().
                Property(p => p.Wert)
                .IsRequired();
            modelBuilder.Entity<Bezahlung>().
                Property(p => p.Zeitpunkt)
                .IsRequired();
            modelBuilder.Entity<Bezahlung>()
                .HasOne(p => p.BezahlendePerson)
                .WithMany(p => p.Bezahlungen)
                .HasForeignKey(p => p.BezahlendePersonId);

            modelBuilder.Entity<Bezahlung>()
                .HasOne(p => p.Kontokorrent)
                .WithMany(p => p.Bezahlungen)
                .HasForeignKey(p => p.KontokorrentId);

            modelBuilder.Entity<Bezahlung>().
                HasMany(p => p.Emfpaenger)
                .WithOne(p => p.Bezahlung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(p => p.BezahlungId);

            modelBuilder.Entity<EmfpaengerInBezahlung>()
                .HasOne(p => p.Empfaenger)
                .WithMany(p => p.EmfpaengerIn)
                .HasForeignKey(p => p.EmpfaengerId);

            modelBuilder.Entity<EmfpaengerInBezahlung>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<EmfpaengerInBezahlung>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<BenutzerSecret>()
                .HasKey(p => p.BenutzerId);

            modelBuilder.Entity<BenutzerSecret>()
                .Property(p => p.HashedSecret)
                .IsRequired();

            modelBuilder.Entity<BenutzerKontokorrent>()
             .HasOne(p => p.Kontokorrent)
             .WithMany(p => p.Benutzer)
             .HasForeignKey(p => p.KontokorrentId);

            modelBuilder.Entity<BenutzerKontokorrent>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<BenutzerKontokorrent>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<BenutzerKontokorrent>()
                .Property(p => p.BenutzerId)
                .IsRequired();

            modelBuilder.Entity<EinladungsCode>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<EinladungsCode>()
                .Property(p => p.GueltigBis)
                .IsRequired();

            modelBuilder.Entity<EinladungsCode>()
                 .HasOne(p => p.Kontokorrent)
                 .WithMany(p => p.EinladungsCodes)
                 .HasForeignKey(p => p.KontokorrentId);
        }
    }
}
