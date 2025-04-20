using FamilyBudgeter.API.BLL.DTOs.BudgetLimitDTOs;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface IBudgetLimitService
    {
        /// <summary>
        /// Отримання всіх лімітів бюджету
        /// </summary>
        Task<IEnumerable<BudgetLimitDto>> GetBudgetLimitsAsync(int budgetId, int userId);

        /// <summary>
        /// Отримання ліміту за ідентифікатором
        /// </summary>
        Task<BudgetLimitDto> GetBudgetLimitByIdAsync(int limitId, int userId);

        /// <summary>
        /// Створення нового ліміту бюджету
        /// </summary>
        Task<BudgetLimitDto> CreateBudgetLimitAsync(CreateBudgetLimitDto limitDto, int userId);

        /// <summary>
        /// Оновлення ліміту бюджету
        /// </summary>
        Task<BudgetLimitDto> UpdateBudgetLimitAsync(int limitId, UpdateBudgetLimitDto limitDto, int userId);

        /// <summary>
        /// Видалення ліміту бюджету
        /// </summary>
        Task<bool> DeleteBudgetLimitAsync(int limitId, int userId);

        /// <summary>
        /// Отримання активних лімітів бюджету на поточну дату
        /// </summary>
        Task<IEnumerable<BudgetLimitDto>> GetActiveBudgetLimitsAsync(int budgetId, int userId);

        /// <summary>
        /// Отримання лімітів за категорією
        /// </summary>
        Task<IEnumerable<BudgetLimitDto>> GetLimitsByCategoryAsync(int categoryId, int userId);

        /// <summary>
        /// Перевірка, чи перевищено ліміт витрат за категорією
        /// </summary>
        Task<bool> IsLimitExceededAsync(int categoryId, int userId);

        /// <summary>
        /// Отримання поточного стану лімітів бюджету
        /// </summary>
        Task<IEnumerable<BudgetLimitDto>> GetLimitStatusesAsync(int budgetId, int userId);
    }
}
