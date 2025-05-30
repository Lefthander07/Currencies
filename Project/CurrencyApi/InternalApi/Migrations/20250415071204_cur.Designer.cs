﻿// <auto-generated />
using System;
using Fuse8.BackendInternship.InternalApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fuse8.BackendInternship.InternalApi.Migrations
{
    [DbContext(typeof(CurrencyDbContext))]
    [Migration("20250415071204_cur")]
    partial class cur
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("cur")
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Fuse8.BackendInternship.InternalApi.Data.CurrencyCache", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BaseCurrency")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)")
                        .HasColumnName("base_currency");

                    b.Property<DateTime>("CacheDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("cache_date");

                    b.HasKey("Id")
                        .HasName("pk_currency_caches");

                    b.ToTable("currency_caches", "cur");
                });

            modelBuilder.Entity("Fuse8.BackendInternship.InternalApi.Data.CurrencyExchange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CurrencyCacheId")
                        .HasColumnType("integer")
                        .HasColumnName("currency_cache_id");

                    b.Property<string>("CurrencyCode")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)")
                        .HasColumnName("currency_code");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("numeric")
                        .HasColumnName("exchange_rate");

                    b.HasKey("Id")
                        .HasName("pk_currency_exchange_rates");

                    b.HasIndex("CurrencyCacheId")
                        .HasDatabaseName("ix_currency_exchange_rates_currency_cache_id");

                    b.ToTable("currency_exchange_rates", "cur");
                });

            modelBuilder.Entity("Fuse8.BackendInternship.InternalApi.Data.CurrencyExchange", b =>
                {
                    b.HasOne("Fuse8.BackendInternship.InternalApi.Data.CurrencyCache", "CurrencyCache")
                        .WithMany("ExchangeRates")
                        .HasForeignKey("CurrencyCacheId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_currency_exchange_rates_currency_caches_currency_cache_id");

                    b.Navigation("CurrencyCache");
                });

            modelBuilder.Entity("Fuse8.BackendInternship.InternalApi.Data.CurrencyCache", b =>
                {
                    b.Navigation("ExchangeRates");
                });
#pragma warning restore 612, 618
        }
    }
}
