using FamilyBudgeter.API.Domain.Entities;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface IBudgetLimitRepository : IGenericRepository<BudgetLimit>
    {
        /// <summary>
        /// Отримати всі ліміти бюджету
        /// </summary>
        Task<IEnumerable<BudgetLimit>> GetByBudgetIdAsync(int budgetId);

        /// <summary>
        /// Отримати всі ліміти за категорією
        /// </summary>
        Task<IEnumerable<BudgetLimit>> GetByCategoryIdAsync(int categoryId);

        /// <summary>
        /// Отримати активні ліміти на поточний період
        /// </summary>
        Task<IEnumerable<BudgetLimit>> GetActiveAsync(int budgetId, DateTime date);
    }
}
