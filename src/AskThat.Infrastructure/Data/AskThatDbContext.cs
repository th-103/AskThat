using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskThat.Domain.Entities;
using AskThat.Domain.Entitites;
using Microsoft.EntityFrameworkCore;


namespace AskThat.Infrastructure.Data
{
    public class AskThatDbContext : DbContext
    {
        public AskThatDbContext(DbContextOptions<AskThatDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.HasData(
                    new Role { RoleId = 1, Name = "User" },
                    new Role { RoleId = 2, Name = "Moderator" },
                    new Role { RoleId = 3, Name = "Admin" }
                );
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasIndex(e => e.Username)
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.QuestionId);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.AnswerCount)
                    .HasDefaultValue(0);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Questions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_Questions_CreatedAt");
                entity.HasIndex(e => new { e.UserId, e.CreatedAt })
                    .HasDatabaseName("IX_Questions_UserId_CreatedAt");
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => e.AnswerId);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Question)
                    .WithMany(q => q.Answers)
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Answers)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.QuestionId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_Answers_CreatedAt");
                entity.HasIndex(e => new { e.QuestionId, e.CreatedAt })
                    .HasDatabaseName("IX_Answers_QuestionId_CreatedAt");
                entity.HasIndex(e => new { e.UserId, e.CreatedAt })
                    .HasDatabaseName("IX_Answers_UserId_CreatedAt");
            });
        }
    }
}