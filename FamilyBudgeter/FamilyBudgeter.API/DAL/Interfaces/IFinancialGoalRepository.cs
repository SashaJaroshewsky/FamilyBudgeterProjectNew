using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface IFinancialGoalRepository : IGenericRepository<FinancialGoal>
    {
        /// <summary>
        /// Отримати всі фінансові цілі бюджету
        /// </summary>
        Task<IEnumerable<FinancialGoal>> GetByBudgetIdAsync(int budgetId);

        /// <summary>
        /// Отримати фінансові цілі за статусом
        /// </summary>
        Task<IEnumerable<FinancialGoal>> GetByStatusAsync(int budgetId, FinancialGoalStatus status);
    }
}
