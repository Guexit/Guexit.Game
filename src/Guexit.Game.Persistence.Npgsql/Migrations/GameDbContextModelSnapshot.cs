﻿// <auto-generated />
using System;
using Guexit.Game.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    [DbContext(typeof(GameDbContext))]
    partial class GameDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("GameRoomId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PlayerHandId")
                        .HasColumnType("uuid");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GameRoomId");

                    b.HasIndex("PlayerHandId");

                    b.ToTable("Cards", (string)null);
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.FinishedRound", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("GameRoomId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("GameRoomId");

                    b.ToTable("FinishedRounds", (string)null);
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.GameRoom", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PlayerIds")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RequiredMinPlayers")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.ToTable("GameRooms");
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.PlayerHand", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GameRoomId")
                        .HasColumnType("uuid");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GameRoomId", "PlayerId")
                        .IsUnique();

                    b.ToTable("PlayerHands", (string)null);
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.SubmittedCard", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CardId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("FinishedRoundId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GameRoomId")
                        .HasColumnType("uuid");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Voters")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.HasIndex("FinishedRoundId");

                    b.HasIndex("GameRoomId", "PlayerId")
                        .IsUnique();

                    b.ToTable("SubmittedCards", (string)null);
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.ImageAggregate.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("GameRoomId")
                        .HasColumnType("uuid");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.PlayerAggregate.Player", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("character varying(320)");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.InboxState", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("Consumed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ConsumerId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Delivered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ExpirationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("LastSequenceNumber")
                        .HasColumnType("bigint");

                    b.Property<Guid>("LockId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid");

                    b.Property<int>("ReceiveCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Received")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasAlternateKey("MessageId", "ConsumerId");

                    b.HasIndex("Delivered");

                    b.ToTable("InboxState");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxMessage", b =>
                {
                    b.Property<long>("SequenceNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("SequenceNumber"));

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<Guid?>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CorrelationId")
                        .HasColumnType("uuid");

                    b.Property<string>("DestinationAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("EnqueueTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ExpirationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FaultAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Headers")
                        .HasColumnType("text");

                    b.Property<Guid?>("InboxConsumerId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("InboxMessageId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("InitiatorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("OutboxId")
                        .HasColumnType("uuid");

                    b.Property<string>("Properties")
                        .HasColumnType("text");

                    b.Property<Guid?>("RequestId")
                        .HasColumnType("uuid");

                    b.Property<string>("ResponseAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("SentTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SourceAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("SequenceNumber");

                    b.HasIndex("EnqueueTime");

                    b.HasIndex("ExpirationTime");

                    b.HasIndex("OutboxId", "SequenceNumber")
                        .IsUnique();

                    b.HasIndex("InboxMessageId", "InboxConsumerId", "SequenceNumber")
                        .IsUnique();

                    b.ToTable("OutboxMessage");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxState", b =>
                {
                    b.Property<Guid>("OutboxId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("Delivered")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("LastSequenceNumber")
                        .HasColumnType("bigint");

                    b.Property<Guid>("LockId")
                        .HasColumnType("uuid");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.HasKey("OutboxId");

                    b.HasIndex("Created");

                    b.ToTable("OutboxState");
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.Card", b =>
                {
                    b.HasOne("Guexit.Game.Domain.Model.GameRoomAggregate.GameRoom", null)
                        .WithMany("Deck")
                        .HasForeignKey("GameRoomId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Guexit.Game.Domain.Model.GameRoomAggregate.PlayerHand", null)
                        .WithMany("Cards")
                        .HasForeignKey("PlayerHandId");
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.FinishedRound", b =>
                {
                    b.HasOne("Guexit.Game.Domain.Model.GameRoomAggregate.GameRoom", null)
                        .WithMany("FinishedRounds")
                        .HasForeignKey("GameRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Guexit.Game.Domain.Model.GameRoomAggregate.Score", "Scores", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<Guid>("FinishedRoundId")
                                .HasColumnType("uuid");

                            b1.Property<string>("PlayerId")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<int>("Points")
                                .HasColumnType("integer");

                            b1.HasKey("Id");

                            b1.HasIndex("FinishedRoundId");

                            b1.ToTable("Scores", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("FinishedRoundId");
                        });

                    b.Navigation("Scores");
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.GameRoom", b =>
                {
                    b.OwnsOne("Guexit.Game.Domain.Model.GameRoomAggregate.StoryTeller", "CurrentStoryTeller", b1 =>
                        {
                            b1.Property<Guid>("GameRoomId")
                                .HasColumnType("uuid");

                            b1.Property<string>("PlayerId")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Story")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("GameRoomId");

                            b1.ToTable("GameRooms");

                            b1.WithOwner()
                                .HasForeignKey("GameRoomId");
                        });

                    b.Navigation("CurrentStoryTeller")
                        .IsRequired();
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.PlayerHand", b =>
                {
                    b.HasOne("Guexit.Game.Domain.Model.GameRoomAggregate.GameRoom", null)
                        .WithMany("PlayerHands")
                        .HasForeignKey("GameRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.SubmittedCard", b =>
                {
                    b.HasOne("Guexit.Game.Domain.Model.GameRoomAggregate.Card", "Card")
                        .WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Guexit.Game.Domain.Model.GameRoomAggregate.FinishedRound", null)
                        .WithMany("SubmittedCards")
                        .HasForeignKey("FinishedRoundId");

                    b.HasOne("Guexit.Game.Domain.Model.GameRoomAggregate.GameRoom", null)
                        .WithMany("SubmittedCards")
                        .HasForeignKey("GameRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.FinishedRound", b =>
                {
                    b.Navigation("SubmittedCards");
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.GameRoom", b =>
                {
                    b.Navigation("Deck");

                    b.Navigation("FinishedRounds");

                    b.Navigation("PlayerHands");

                    b.Navigation("SubmittedCards");
                });

            modelBuilder.Entity("Guexit.Game.Domain.Model.GameRoomAggregate.PlayerHand", b =>
                {
                    b.Navigation("Cards");
                });
#pragma warning restore 612, 618
        }
    }
}
