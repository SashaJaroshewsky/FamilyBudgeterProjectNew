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
                .HasForeignKey(fm => fm.FamilyId);

            // Budget
            modelBuilder.Entity<Budget>()
                .HasOne(b => b.Family)
                .WithMany(f => f.Budgets)
                .HasForeignKey(b => b.FamilyId);

            // Category
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Budget)
                .WithMany(b => b.Categories)
                .HasForeignKey(c => c.BudgetId);

            // Transaction
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Budget)
                .WithMany(b => b.Transactions)
                .HasForeignKey(t => t.BudgetId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BudgetLimit
            modelBuilder.Entity<BudgetLimit>()
                .HasOne(bl => bl.Category)
                .WithMany(c => c.BudgetLimits)
                .HasForeignKey(bl => bl.CategoryId);

            modelBuilder.Entity<BudgetLimit>()
                .HasOne(bl => bl.Budget)
                .WithMany(b => b.BudgetLimits)
                .HasForeignKey(bl => bl.BudgetId);

            // FinancialGoal
            modelBuilder.Entity<FinancialGoal>()
                .HasOne(fg => fg.Budget)
                .WithMany(b => b.FinancialGoals)
                .HasForeignKey(fg => fg.BudgetId);

            // Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Family)
                .WithMany()
                .HasForeignKey(n => n.FamilyId)
                .IsRequired(false);

            // RegularPayment
            modelBuilder.Entity<RegularPayment>()
                .HasOne(rp => rp.Category)
                .WithMany()
                .HasForeignKey(rp => rp.CategoryId);

            modelBuilder.Entity<RegularPayment>()
                .HasOne(rp => rp.Budget)
                .WithMany()
                .HasForeignKey(rp => rp.BudgetId);
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
