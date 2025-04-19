using FamilyBudgeter.API.Domain.Entities;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        /// <summary>
        /// Отримати всі транзакції бюджету
        /// </summary>
        Task<IEnumerable<Transaction>> GetByBudgetIdAsync(int budgetId);

        /// <summary>
        /// Отримати транзакції за категорією
        /// </summary>
        Task<IEnumerable<Transaction>> GetByCategoryIdAsync(int categoryId);

        /// <summary>
        /// Отримати транзакції за користувачем
        /// </summary>
        Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Отримати транзакції за діапазоном дат
        /// </summary>
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(int budgetId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Отримати суму транзакцій за категорією
        /// </summary>
        Task<decimal> GetSumByCategoryAsync(int categoryId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
