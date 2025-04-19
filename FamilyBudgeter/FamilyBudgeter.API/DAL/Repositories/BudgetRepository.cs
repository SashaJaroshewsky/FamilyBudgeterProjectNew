using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class BudgetRepository : GenericRepository<Budget>, IBudgetRepository
    {
        public BudgetRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Budget>> GetByFamilyIdAsync(int familyId)
        {
            return await _dbSet
                .Where(b => b.FamilyId == familyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Budget>> GetByTypeAsync(int familyId, BudgetType type)
        {
            return await _dbSet
                .Where(b => b.FamilyId == familyId && b.Type == type)
                .ToListAsync();
        }

        public async Task<Budget?> GetWithCategoriesAsync(int budgetId)
        {
            return await _dbSet
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == budgetId);
        }

        public async Task<Budget?> GetWithTransactionsAsync(int budgetId)
        {
            return await _dbSet
                .Include(b => b.Transactions)
                    .ThenInclude(t => t.Category)
                .Include(b => b.Transactions)
                    .ThenInclude(t => t.CreatedByUser)
                .FirstOrDefaultAsync(b => b.Id == budgetId);
        }

        public async Task<Budget?> GetWithFinancialGoalsAsync(int budgetId)
        {
            return await _dbSet
                .Include(b => b.FinancialGoals)
                .FirstOrDefaultAsync(b => b.Id == budgetId);
        }
    }
}
