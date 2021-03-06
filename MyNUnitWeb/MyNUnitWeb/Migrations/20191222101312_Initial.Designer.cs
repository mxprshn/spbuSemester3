﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Migrations
{
    [DbContext(typeof(TestArchive))]
    [Migration("20191222101312_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MyNUnitWeb.Models.AssemblyModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AssemblyModels");
                });

            modelBuilder.Entity("MyNUnitWeb.Models.TestModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AssemblyModelId")
                        .HasColumnType("int");

                    b.Property<string>("ClassName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IgnoreReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsIgnored")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsPassed")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("RunTime")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("AssemblyModelId");

                    b.ToTable("TestModels");
                });

            modelBuilder.Entity("MyNUnitWeb.Models.TestModel", b =>
                {
                    b.HasOne("MyNUnitWeb.Models.AssemblyModel", "AssemblyModel")
                        .WithMany("TestModels")
                        .HasForeignKey("AssemblyModelId");
                });
#pragma warning restore 612, 618
        }
    }
}
