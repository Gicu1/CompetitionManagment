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

            entity.HasOne(d => d.CompetitionTypeNavigation).WithMany(p => p.Competitions).HasConstraintName("FK__COMPETITI__Compe__4D94879B");

            entity.HasMany(d => d.Teams).WithMany(p => p.Competitions)
                .UsingEntity<Dictionary<string, object>>(
                    "Teamcompetition",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TEAMCOMPE__TeamI__5CD6CB2B"),
                    l => l.HasOne<Competition>().WithMany()
                        .HasForeignKey("CompetitionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TEAMCOMPE__Compe__5BE2A6F2"),
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

            entity.HasOne(d => d.Competition).WithMany(p => p.Games).HasConstraintName("FK__GAME__Competitio__52593CB8");

            entity.HasOne(d => d.Team1).WithMany(p => p.GameTeam1s).HasConstraintName("FK__GAME__Team1ID__5070F446");

            entity.HasOne(d => d.Team2).WithMany(p => p.GameTeam2s).HasConstraintName("FK__GAME__Team2ID__5165187F");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PLAYER__3214EC27AC49A529");

            entity.HasOne(d => d.Team).WithMany(p => p.Players).HasConstraintName("FK__PLAYER__TeamID__5535A963");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TEAM__3214EC279E2E117C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
