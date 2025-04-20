using FamilyBudgeter.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FamilyBudgeter.API.DAL.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BudgetLimit> BudgetLimits { get; set; }
        public DbSet<FinancialGoal> FinancialGoals { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RegularPayment> RegularPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфігурація типів decimal для всіх властивостей
            // Transaction
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            // BudgetLimit
            modelBuilder.Entity<BudgetLimit>()
                .Property(bl => bl.Amount)
                .HasColumnType("decimal(18,2)");

            // FinancialGoal
            modelBuilder.Entity<FinancialGoal>()
                .Property(fg => fg.TargetAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<FinancialGoal>()
                .Property(fg => fg.CurrentAmount)
                .HasColumnType("decimal(18,2)");

            // RegularPayment
            modelBuilder.Entity<RegularPayment>()
                .Property(rp => rp.Amount)
                .HasColumnType("decimal(18,2)");

            // Конфігурація сутностей
            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Family
            modelBuilder.Entity<Family>()
                .HasIndex(f => f.JoinCode)
                .IsUnique();

            // FamilyMember
            modelBuilder.Entity<FamilyMember>()
                .HasOne(fm => fm.User)
                .WithMany(u => u.FamilyMemberships)
                .HasForeignKey(fm => fm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FamilyMember>()
                .HasOne(fm => fm.Family)
                .WithMany(f => f.Members)
                .HasForeignKey(fm => fm.FamilyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Budget
            modelBuilder.Entity<Budget>()
                .HasOne(b => b.Family)
                .WithMany(f => f.Budgets)
                .HasForeignKey(b => b.FamilyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Budget)
                .WithMany(b => b.Categories)
                .HasForeignKey(c => c.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Transaction
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Змінено з CASCADE на RESTRICT

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Budget)
                .WithMany(b => b.Transactions)
                .HasForeignKey(t => t.BudgetId)
                .OnDelete(DeleteBehavior.Restrict); // Змінено з CASCADE на RESTRICT

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BudgetLimit
            modelBuilder.Entity<BudgetLimit>()
                .HasOne(bl => bl.Category)
                .WithMany(c => c.BudgetLimits)
                .HasForeignKey(bl => bl.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Змінено з CASCADE на RESTRICT

            modelBuilder.Entity<BudgetLimit>()
                .HasOne(bl => bl.Budget)
                .WithMany(b => b.BudgetLimits)
                .HasForeignKey(bl => bl.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            // FinancialGoal
            modelBuilder.Entity<FinancialGoal>()
                .HasOne(fg => fg.Budget)
                .WithMany(b => b.FinancialGoals)
                .HasForeignKey(fg => fg.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Family)
                .WithMany()
                .HasForeignKey(n => n.FamilyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // Змінено з CASCADE на RESTRICT

            // RegularPayment
            modelBuilder.Entity<RegularPayment>()
                .HasOne(rp => rp.Category)
                .WithMany()
                .HasForeignKey(rp => rp.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Змінено з CASCADE на RESTRICT

            modelBuilder.Entity<RegularPayment>()
                .HasOne(rp => rp.Budget)
                .WithMany()
                .HasForeignKey(rp => rp.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                else
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
