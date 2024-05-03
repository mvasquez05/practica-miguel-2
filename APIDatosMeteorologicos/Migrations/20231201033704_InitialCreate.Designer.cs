﻿// <auto-generated />
using System;
using APIDatosMeteorologicos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace APIDatosMeteorologicos.Migrations
{
    [DbContext(typeof(APIDatosMeteorologicosContext))]
    [Migration("20231201033704_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("APIDatosMeteorologicos.Models.RegistroMeteorologico", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<double>("Humedad")
                        .HasColumnType("float");

                    b.Property<double>("Precipitacion")
                        .HasColumnType("float");

                    b.Property<double>("Temperatura")
                        .HasColumnType("float");

                    b.Property<double>("VelocidadViento")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("RegistrosMeteorologicos");
                });
#pragma warning restore 612, 618
        }
    }
}
