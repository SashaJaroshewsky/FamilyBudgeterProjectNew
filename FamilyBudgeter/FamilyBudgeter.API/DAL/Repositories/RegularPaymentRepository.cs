using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class RegularPaymentRepository : GenericRepository<RegularPayment>, IRegularPaymentRepository
    {
        public RegularPaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RegularPayment>> GetByBudgetIdAsync(int budgetId)
        {
            return await _dbSet
                .Include(rp => rp.Category)
                .Where(rp => rp.BudgetId == budgetId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RegularPayment>> GetByFrequencyAsync(int budgetId, PaymentFrequency frequency)
        {
            return await _dbSet
                .Include(rp => rp.Category)
                .Where(rp => rp.BudgetId == budgetId && rp.Frequency == frequency)
                .ToListAsync();
        }

        public async Task<IEnumerable<RegularPayment>> GetActiveAsync(int budgetId, DateTime date)
        {
            return await _dbSet
                .Include(rp => rp.Category)
                .Where(rp => rp.BudgetId == budgetId &&
                             rp.StartDate <= date &&
                             (!rp.EndDate.HasValue || rp.EndDate.Value >= date))
                .ToListAsync();
        }
    }
}
