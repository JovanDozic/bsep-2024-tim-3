﻿// <auto-generated />
using System;
using Marketing_system.DA.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Marketing_system.DA.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Marketing_system.DA.Contracts.Model.Advertisement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("Deadline")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("Slogan")
                        .HasColumnType("text");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Advertisements");
                });

            modelBuilder.Entity("Marketing_system.DA.Contracts.Model.PasswordlessToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("boolean");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("PasswordlessTokens");
                });

            modelBuilder.Entity("Marketing_system.DA.Contracts.Model.RegistrationRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountStatus")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PackageType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("RegistrationDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("RegistrationRequests");
                });

            modelBuilder.Entity("Marketing_system.DA.Contracts.Model.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Marketing_system.DA.Contracts.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountStatus")
                        .HasColumnType("integer");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ClientType")
                        .HasColumnType("integer");

                    b.Property<string>("CompanyName")
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Firstname")
                        .HasColumnType("text");

                    b.Property<bool>("IsFirstLogin")
                        .HasColumnType("boolean");

                    b.Property<string>("Lastname")
                        .HasColumnType("text");

                    b.Property<int>("PackageType")
                        .HasColumnType("integer");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("TaxId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
