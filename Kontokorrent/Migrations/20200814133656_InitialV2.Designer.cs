﻿// <auto-generated />
using System;
using Kontokorrent.Impl.EFV2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kontokorrent.Migrations
{
    [DbContext(typeof(KontokorrentV2Context))]
    [Migration("20200814133656_InitialV2")]
    partial class InitialV2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7");

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.Aktion", b =>
                {
                    b.Property<string>("KontokorrentId")
                        .HasColumnType("TEXT");

                    b.Property<int>("LaufendeNummer")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BearbeiteteBezahlungId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BezahlungId")
                        .HasColumnType("TEXT");

                    b.Property<string>("GeloeschteBezahlungId")
                        .HasColumnType("TEXT");

                    b.HasKey("KontokorrentId", "LaufendeNummer");

                    b.HasIndex("BearbeiteteBezahlungId")
                        .IsUnique();

                    b.HasIndex("BezahlungId")
                        .IsUnique();

                    b.HasIndex("GeloeschteBezahlungId")
                        .IsUnique();

                    b.ToTable("Aktionen");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.BenutzerKontokorrent", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("BenutzerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("KontokorrentId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("KontokorrentId");

                    b.ToTable("BenutzerKontokorrent");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.BenutzerSecret", b =>
                {
                    b.Property<string>("BenutzerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("HashedSecret")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("BenutzerId");

                    b.ToTable("BenutzerSecret");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.Bezahlung", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Beschreibung")
                        .HasColumnType("TEXT");

                    b.Property<string>("BezahlendePersonId")
                        .HasColumnType("TEXT");

                    b.Property<double>("Wert")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Zeitpunkt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BezahlendePersonId");

                    b.ToTable("Bezahlung");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.EinladungsCode", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("GueltigBis")
                        .HasColumnType("TEXT");

                    b.Property<string>("KontokorrentId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("KontokorrentId");

                    b.ToTable("EinladungsCode");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.EmfpaengerInBezahlung", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BezahlungId")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmpfaengerId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BezahlungId");

                    b.HasIndex("EmpfaengerId");

                    b.ToTable("EmfpaengerInBezahlung");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.Kontokorrent", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("LaufendeNummer")
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("OeffentlicherName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Privat")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OeffentlicherName")
                        .IsUnique();

                    b.ToTable("Kontokorrent");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.Person", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("KontokorrentId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasAlternateKey("KontokorrentId", "Name");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.Aktion", b =>
                {
                    b.HasOne("Kontokorrent.Impl.EFV2.Bezahlung", "BearbeiteteBezahlung")
                        .WithOne("BearbeitendeAktion")
                        .HasForeignKey("Kontokorrent.Impl.EFV2.Aktion", "BearbeiteteBezahlungId");

                    b.HasOne("Kontokorrent.Impl.EFV2.Bezahlung", "Bezahlung")
                        .WithOne("ErstellendeAktion")
                        .HasForeignKey("Kontokorrent.Impl.EFV2.Aktion", "BezahlungId");

                    b.HasOne("Kontokorrent.Impl.EFV2.Bezahlung", "GeloeschteBezahlung")
                        .WithOne("LoeschendeAktion")
                        .HasForeignKey("Kontokorrent.Impl.EFV2.Aktion", "GeloeschteBezahlungId");

                    b.HasOne("Kontokorrent.Impl.EFV2.Kontokorrent", "Kontokorrent")
                        .WithMany("Aktionen")
                        .HasForeignKey("KontokorrentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.BenutzerKontokorrent", b =>
                {
                    b.HasOne("Kontokorrent.Impl.EFV2.Kontokorrent", "Kontokorrent")
                        .WithMany("Benutzer")
                        .HasForeignKey("KontokorrentId");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.Bezahlung", b =>
                {
                    b.HasOne("Kontokorrent.Impl.EFV2.Person", "BezahlendePerson")
                        .WithMany("Bezahlungen")
                        .HasForeignKey("BezahlendePersonId");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.EinladungsCode", b =>
                {
                    b.HasOne("Kontokorrent.Impl.EFV2.Kontokorrent", "Kontokorrent")
                        .WithMany("EinladungsCodes")
                        .HasForeignKey("KontokorrentId");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.EmfpaengerInBezahlung", b =>
                {
                    b.HasOne("Kontokorrent.Impl.EFV2.Bezahlung", "Bezahlung")
                        .WithMany("Emfpaenger")
                        .HasForeignKey("BezahlungId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Kontokorrent.Impl.EFV2.Person", "Empfaenger")
                        .WithMany("EmfpaengerIn")
                        .HasForeignKey("EmpfaengerId");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EFV2.Person", b =>
                {
                    b.HasOne("Kontokorrent.Impl.EFV2.Kontokorrent", "Kontokorrent")
                        .WithMany("Personen")
                        .HasForeignKey("KontokorrentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
