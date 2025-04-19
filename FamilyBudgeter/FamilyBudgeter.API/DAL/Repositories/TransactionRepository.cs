using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transaction>> GetByBudgetIdAsync(int budgetId)
        {
            return await _dbSet
                .Include(t => t.Category)
                .Include(t => t.CreatedByUser)
                .Where(t => t.BudgetId == budgetId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Include(t => t.CreatedByUser)
                .Where(t => t.CategoryId == categoryId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(t => t.Category)
                .Include(t => t.Budget)
                .Where(t => t.CreatedByUserId == userId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(int budgetId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(t => t.Category)
                .Include(t => t.CreatedByUser)
                .Where(t => t.BudgetId == budgetId && t.Date >= startDate && t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetSumByCategoryAsync(int categoryId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.Where(t => t.CategoryId == categoryId);

            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }

            return await query.SumAsync(t => t.Amount);
        }
    }
}
