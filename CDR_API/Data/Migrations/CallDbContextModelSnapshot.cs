﻿// <auto-generated />
using System;
using CDR_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CDR_API.Data.Migrations
{
    [DbContext(typeof(CallDbContext))]
    partial class CallDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CDR_API.Models.Call", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("call_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("caller_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("currency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("duration")
                        .HasColumnType("int");

                    b.Property<DateTime>("end_time")
                        .HasColumnType("datetime2");

                    b.Property<string>("recipient")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("reference")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("reference")
                        .IsUnique();

                    b.ToTable("Calls");
                });
#pragma warning restore 612, 618
        }
    }
}