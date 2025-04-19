using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class FinancialGoalRepository : GenericRepository<FinancialGoal>, IFinancialGoalRepository
    {
        public FinancialGoalRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FinancialGoal>> GetByBudgetIdAsync(int budgetId)
        {
            return await _dbSet
                .Where(fg => fg.BudgetId == budgetId)
                .OrderBy(fg => fg.Deadline)
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialGoal>> GetByStatusAsync(int budgetId, FinancialGoalStatus status)
        {
            return await _dbSet
                .Where(fg => fg.BudgetId == budgetId && fg.Status == status)
                .OrderBy(fg => fg.Deadline)
                .ToListAsync();
        }
    }
}
