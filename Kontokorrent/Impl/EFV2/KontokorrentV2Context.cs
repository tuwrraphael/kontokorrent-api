using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Kontokorrent.Impl.EFV2
{
    public class Kontokorrent
    {
        public string Id { get; set; }
        public string OeffentlicherName { get; set; }
        public List<Person> Personen { get; set; }
        public List<BenutzerKontokorrent> Benutzer { get; set; }
        public string Name { get; set; }
        public bool Privat { get; set; }
        public List<EinladungsCode> EinladungsCodes { get; set; }
        public int LaufendeNummer { get; set; }
        public List<Aktion> Aktionen { get; set; }
    }

    public class Aktion
    {
        public string KontokorrentId { get; set; }
        public int LaufendeNummer { get; set; }
        public Kontokorrent Kontokorrent { get; set; }
        public string BezahlungId { get; set; }
        public Bezahlung Bezahlung { get; set; }
        public string GeloeschteBezahlungId { get; set; }
        public Bezahlung GeloeschteBezahlung { get; set; }
        public string BearbeiteteBezahlungId { get; set; }
        public Bezahlung BearbeiteteBezahlung { get; set; }
    }

    public class EinladungsCode
    {
        public string Id { get; set; }
        public DateTime GueltigBis { get; set; }
        public Kontokorrent Kontokorrent { get; set; }
        public string KontokorrentId { get; set; }
    }

    public class BenutzerSecret
    {
        public string BenutzerId { get; set; }
        public string HashedSecret { get; set; }
    }

    public class BenutzerKontokorrent
    {
        public string Id { get; set; }
        public string BenutzerId { get; set; }

        public string KontokorrentId { get; set; }
        public Kontokorrent Kontokorrent { get; set; }
    }

    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string KontokorrentId { get; set; }
        public Kontokorrent Kontokorrent { get; set; }
        public List<Bezahlung> Bezahlungen { get; set; }
        public List<EmfpaengerInBezahlung> EmfpaengerIn { get; set; }
    }
    public class EmfpaengerInBezahlung
    {
        public int Id { get; set; }
        public string EmpfaengerId { get; set; }
        public Person Empfaenger { get; set; }
        public string BezahlungId { get; set; }
        public Bezahlung Bezahlung { get; set; }
    }
    public class Bezahlung
    {
        public string Id { get; set; }
        public string BezahlendePersonId { get; set; }
        public Person BezahlendePerson { get; set; }
        public List<EmfpaengerInBezahlung> Emfpaenger { get; set; }
        public double Wert { get; set; }
        public string Beschreibung { get; set; }
        public DateTime Zeitpunkt { get; set; }
        public Aktion ErstellendeAktion { get; set; }
        public Aktion LoeschendeAktion { get; set; }
        public Aktion BearbeitendeAktion { get; set; }
    }
    public class KontokorrentV2Context : DbContext
    {
        public KontokorrentV2Context(DbContextOptions<KontokorrentV2Context> contextOptions) : base(contextOptions)
        {

        }

        public DbSet<Kontokorrent> Kontokorrent { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Bezahlung> Bezahlung { get; set; }
        public DbSet<Aktion> Aktionen { get; set; }
        public DbSet<EinladungsCode> EinladungsCode { get; set; }
        public DbSet<BenutzerSecret> BenutzerSecret { get; set; }
        public DbSet<BenutzerKontokorrent> BenutzerKontokorrent { get; set; }

        public DbSet<EmfpaengerInBezahlung> EmfpaengerInBezahlung { get; set; }

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
                .IsRequired(false);
            modelBuilder.Entity<Kontokorrent>()
                .Property(p => p.LaufendeNummer)
                .HasDefaultValue(0)
                .ValueGeneratedNever()
                .IsRequired();
            modelBuilder.Entity<Kontokorrent>().
                HasIndex(p => p.OeffentlicherName)
                .IsUnique(true);

            modelBuilder.Entity<Person>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Person>().
                Property(p => p.Name)
                .IsRequired();
            modelBuilder.Entity<Person>().
                HasAlternateKey(p => new { p.KontokorrentId, p.Name });

            modelBuilder.Entity<Aktion>()
                .HasKey(p => new { p.KontokorrentId, p.LaufendeNummer });
            modelBuilder.Entity<Aktion>()
                .Property(p => p.LaufendeNummer)
                .ValueGeneratedNever();
            modelBuilder.Entity<Aktion>()
                .HasOne(a => a.Kontokorrent)
                .WithMany(a => a.Aktionen)
                .HasForeignKey(a => a.KontokorrentId)
                .IsRequired(true);
            modelBuilder.Entity<Aktion>()
                .HasOne(a => a.Bezahlung)
                .WithOne(a => a.ErstellendeAktion)
                .HasForeignKey<Aktion>(a => a.BezahlungId)
                .IsRequired(false);
            modelBuilder.Entity<Aktion>()
                    .HasOne(a => a.GeloeschteBezahlung)
                    .WithOne(a => a.LoeschendeAktion)
                    .HasForeignKey<Aktion>(a => a.GeloeschteBezahlungId)
                    .IsRequired(false);
            modelBuilder.Entity<Aktion>()
                    .HasOne(a => a.BearbeiteteBezahlung)
                    .WithOne(a => a.BearbeitendeAktion)
                    .HasForeignKey<Aktion>(a => a.BearbeiteteBezahlungId)
                    .IsRequired(false);
            modelBuilder.Entity<Aktion>()
                .HasIndex(v => v.BezahlungId)
                .IsUnique(true);
            modelBuilder.Entity<Aktion>()
                .HasIndex(v => v.BearbeiteteBezahlungId)
                .IsUnique(true);
            modelBuilder.Entity<Aktion>()
                .HasIndex(v => v.GeloeschteBezahlungId)
                .IsUnique(true);

            modelBuilder.Entity<Bezahlung>().
                HasKey(p => p.Id);
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
