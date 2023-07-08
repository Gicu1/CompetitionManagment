using System;
using System.Collections.Generic;
using CompetitionManagment.Models;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagment.Data;

public partial class CompetitionManagementContext : DbContext
{
    public CompetitionManagementContext()
    {
    }

    public CompetitionManagementContext(DbContextOptions<CompetitionManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<Competitiontype> Competitiontypes { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-2UTOLST; Initial catalog=CompetitionManagement; trusted_connection=yes; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__COMPETIT__3214EC27C1B5A6A3");

            entity.HasOne(d => d.CompetitionTypeNavigation).WithMany(p => p.Competitions)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_COMPETITION_CompetitionType_COMPETITIONTYPE_ID");

            entity.HasMany(d => d.Teams).WithMany(p => p.Competitions)
                .UsingEntity<Dictionary<string, object>>(
                    "Teamcompetition",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .HasConstraintName("FK_TEAMCOMPETITION_TeamID_TEAM_ID"),
                    l => l.HasOne<Competition>().WithMany()
                        .HasForeignKey("CompetitionId")
                        .HasConstraintName("FK_TEAMCOMPETITION_CompetitionID_COMPETITION_ID"),
                    j =>
                    {
                        j.HasKey("CompetitionId", "TeamId").HasName("PK_TeamCompetition");
                        j.ToTable("TEAMCOMPETITION");
                        j.IndexerProperty<int>("CompetitionId").HasColumnName("CompetitionID");
                        j.IndexerProperty<int>("TeamId").HasColumnName("TeamID");
                    });
        });

        modelBuilder.Entity<Competitiontype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TYPE__3214EC2776DBA1B4");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GAME__3214EC2724452D1E");

            entity.HasOne(d => d.Competition).WithMany(p => p.Games)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_GAME_CompetitionID_COMPETITION_ID");

            entity.HasOne(d => d.Team1).WithMany(p => p.GameTeam1s).HasConstraintName("FK_GAME_Team1ID_TEAM_ID");

            entity.HasOne(d => d.Team2).WithMany(p => p.GameTeam2s).HasConstraintName("FK_GAME_Team2ID_TEAM_ID");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PLAYER__3214EC27AC49A529");

            entity.HasOne(d => d.Team).WithMany(p => p.Players)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PLAYER_TeamID_TEAM_ID");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TEAM__3214EC279E2E117C");

            entity.ToTable("TEAM", tb => tb.HasTrigger("TR_TEAM_DELETE_GAME"));
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
