using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class BudgetLimitRepository : GenericRepository<BudgetLimit>, IBudgetLimitRepository
    {
        public BudgetLimitRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BudgetLimit>> GetByBudgetIdAsync(int budgetId)
        {
            return await _dbSet
                .Include(bl => bl.Category)
                .Where(bl => bl.BudgetId == budgetId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BudgetLimit>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Where(bl => bl.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BudgetLimit>> GetActiveAsync(int budgetId, DateTime date)
        {
            return await _dbSet
                .Include(bl => bl.Category)
                .Where(bl => bl.BudgetId == budgetId &&
                             bl.StartDate <= date &&
                             bl.EndDate >= date)
                .ToListAsync();
        }
    }
}
