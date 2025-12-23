using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Self_care.Models;

namespace Self_care.Data;

public partial class SelfCareDB : DbContext
{
    public SelfCareDB()
    {
    }

    public SelfCareDB(DbContextOptions<SelfCareDB> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Habit> Habits { get; set; }

    public virtual DbSet<HabitLog> HabitLogs { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserHabit> UserHabits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=self_careDB;Username=postgres;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("articles_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Habit>(entity =>
        {
            entity.HasKey(e => e.HabitsId).HasName("habits_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<HabitLog>(entity =>
        {
            entity.HasKey(e => e.HabitLgId).HasName("habit_logs_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Deleted).HasDefaultValue(false);

            entity.HasOne(d => d.UserHabit).WithMany(p => p.HabitLogs).HasConstraintName("habit_logs_user_habit_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("sessions_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Deleted).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Sessions).HasConstraintName("sessions_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Deleted).HasDefaultValue(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<UserHabit>(entity =>
        {
            entity.HasKey(e => e.UsHabitId).HasName("user_habits_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.IsMarked).HasDefaultValue(false);

            entity.HasOne(d => d.Habit).WithMany(p => p.UserHabits).HasConstraintName("user_habits_habit_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserHabits).HasConstraintName("user_habits_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
