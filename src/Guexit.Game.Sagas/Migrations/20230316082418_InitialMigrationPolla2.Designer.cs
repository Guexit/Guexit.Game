﻿// <auto-generated />
using System;
using Guexit.Game.Sagas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Guexit.Game.Sagas.Migrations
{
    [DbContext(typeof(DeckAssignmentSagaDbContext))]
    [Migration("20230316082418_InitialMigrationPolla2")]
    partial class InitialMigrationPolla2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Guexit.Game.Sagas.DeckAssignmentState", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid");

                    b.Property<string>("CurrentState")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LogicalShard")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Version")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("CorrelationId");

                    b.ToTable("DeckAssignmentState");
                });
#pragma warning restore 612, 618
        }
    }
}
