﻿// <auto-generated />
using System;
using Kontokorrent.Impl.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kontokorrent.Migrations
{
    [DbContext(typeof(KontokorrentContext))]
    [Migration("20181102154121_SoftDelete")]
    partial class SoftDelete
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("Kontokorrent.Impl.EF.Bezahlung", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Beschreibung");

                    b.Property<string>("BezahlendePersonId");

                    b.Property<bool>("Deleted");

                    b.Property<string>("KontokorrentId")
                        .IsRequired();

                    b.Property<double>("Wert");

                    b.Property<DateTime>("Zeitpunkt");

                    b.HasKey("Id");

                    b.HasIndex("BezahlendePersonId");

                    b.HasIndex("KontokorrentId");

                    b.ToTable("Bezahlung");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EF.EmfpaengerInBezahlung", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BezahlungId");

                    b.Property<string>("EmpfaengerId");

                    b.HasKey("Id");

                    b.HasIndex("BezahlungId");

                    b.HasIndex("EmpfaengerId");

                    b.ToTable("EmfpaengerInBezahlung");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EF.Kontokorrent", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Secret")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("Secret");

                    b.ToTable("Kontokorrent");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EF.Person", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("KontokorrentId")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("KontokorrentId", "Name");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EF.Bezahlung", b =>
                {
                    b.HasOne("Kontokorrent.Impl.EF.Person", "BezahlendePerson")
                        .WithMany("Bezahlungen")
                        .HasForeignKey("BezahlendePersonId");

                    b.HasOne("Kontokorrent.Impl.EF.Kontokorrent", "Kontokorrent")
                        .WithMany("Bezahlungen")
                        .HasForeignKey("KontokorrentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Kontokorrent.Impl.EF.EmfpaengerInBezahlung", b =>
                {
                    b.HasOne("Kontokorrent.Impl.EF.Bezahlung", "Bezahlung")
                        .WithMany("Emfpaenger")
                        .HasForeignKey("BezahlungId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Kontokorrent.Impl.EF.Person", "Empfaenger")
                        .WithMany("EmfpaengerIn")
                        .HasForeignKey("EmpfaengerId");
                });

            modelBuilder.Entity("Kontokorrent.Impl.EF.Person", b =>
                {
                    b.HasOne("Kontokorrent.Impl.EF.Kontokorrent", "Kontokorrent")
                        .WithMany("Personen")
                        .HasForeignKey("KontokorrentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
