using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetByBudgetIdAsync(int budgetId)
        {
            return await _dbSet
                .Where(c => c.BudgetId == budgetId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetByTypeAsync(int budgetId, CategoryType type)
        {
            return await _dbSet
                .Where(c => c.BudgetId == budgetId && c.Type == type)
                .ToListAsync();
        }

        public async Task<Category?> GetWithTransactionsAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Transactions)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
}
