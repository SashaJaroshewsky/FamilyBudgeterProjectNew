using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface IBudgetRepository : IGenericRepository<Budget>
    {
        /// <summary>
        /// Отримати всі бюджети сім'ї
        /// </summary>
        Task<IEnumerable<Budget>> GetByFamilyIdAsync(int familyId);

        /// <summary>
        /// Отримати бюджети за типом
        /// </summary>
        Task<IEnumerable<Budget>> GetByTypeAsync(int familyId, BudgetType type);

        /// <summary>
        /// Отримати бюджет з усіма категоріями
        /// </summary>
        Task<Budget?> GetWithCategoriesAsync(int budgetId);

        /// <summary>
        /// Отримати бюджет з усіма транзакціями
        /// </summary>
        Task<Budget?> GetWithTransactionsAsync(int budgetId);

        /// <summary>
        /// Отримати бюджет з усіма фінансовими цілями
        /// </summary>
        Task<Budget?> GetWithFinancialGoalsAsync(int budgetId);
    }
}
