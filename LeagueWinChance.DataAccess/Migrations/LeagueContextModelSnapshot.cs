﻿// <auto-generated />
using System;
using LeagueWinChance.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LeagueWinChance.DataAccess.Migrations
{
    [DbContext(typeof(LeagueContext))]
    partial class LeagueContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("LeagueWinChance.DataAccess.Models.LeagueMatch", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("Duration")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("MatchDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TeamWinId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("LeagueWinChance.DataAccess.Models.MatchPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Lane")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LeagueMatchId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SummonerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LeagueMatchId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("LeagueWinChance.DataAccess.Models.Stats", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<long>("Assists")
                        .HasColumnType("bigint");

                    b.Property<string>("ChampionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Deaths")
                        .HasColumnType("bigint");

                    b.Property<long>("Kills")
                        .HasColumnType("bigint");

                    b.Property<int>("MatchPlayerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MatchPlayerId")
                        .IsUnique();

                    b.ToTable("Stats");
                });

            modelBuilder.Entity("LeagueWinChance.DataAccess.Models.TgUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<int>("Region")
                        .HasColumnType("int");

                    b.Property<string>("SummonerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TgId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("TgUsers");
                });

            modelBuilder.Entity("LeagueWinChance.DataAccess.Models.MatchPlayer", b =>
                {
                    b.HasOne("LeagueWinChance.DataAccess.Models.LeagueMatch", null)
                        .WithMany("Players")
                        .HasForeignKey("LeagueMatchId");
                });

            modelBuilder.Entity("LeagueWinChance.DataAccess.Models.Stats", b =>
                {
                    b.HasOne("LeagueWinChance.DataAccess.Models.MatchPlayer", "MatchPlayer")
                        .WithOne("Stats")
                        .HasForeignKey("LeagueWinChance.DataAccess.Models.Stats", "MatchPlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MatchPlayer");
                });

            modelBuilder.Entity("LeagueWinChance.DataAccess.Models.LeagueMatch", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("LeagueWinChance.DataAccess.Models.MatchPlayer", b =>
                {
                    b.Navigation("Stats")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
