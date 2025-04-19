using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        /// <summary>
        /// Отримати всі категорії бюджету
        /// </summary>
        Task<IEnumerable<Category>> GetByBudgetIdAsync(int budgetId);

        /// <summary>
        /// Отримати категорії за типом
        /// </summary>
        Task<IEnumerable<Category>> GetByTypeAsync(int budgetId, CategoryType type);

        /// <summary>
        /// Отримати категорію з усіма транзакціями
        /// </summary>
        Task<Category?> GetWithTransactionsAsync(int categoryId);
    }
}
