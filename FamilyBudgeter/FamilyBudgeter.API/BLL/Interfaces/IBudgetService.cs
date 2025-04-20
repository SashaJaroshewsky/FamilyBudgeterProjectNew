using FamilyBudgeter.API.BLL.DTOs.BudgetDTOs;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface IBudgetService
    {
        /// <summary>
        /// Отримання всіх бюджетів сім'ї
        /// </summary>
        Task<IEnumerable<BudgetDto>> GetFamilyBudgetsAsync(int familyId, int userId);

        /// <summary>
        /// Отримання бюджету за ідентифікатором
        /// </summary>
        Task<BudgetDetailDto> GetBudgetByIdAsync(int budgetId, int userId);

        /// <summary>
        /// Створення нового бюджету
        /// </summary>
        Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto budgetDto, int userId);

        /// <summary>
        /// Оновлення бюджету
        /// </summary>
        Task<BudgetDto> UpdateBudgetAsync(int budgetId, UpdateBudgetDto budgetDto, int userId);

        /// <summary>
        /// Видалення бюджету
        /// </summary>
        Task<bool> DeleteBudgetAsync(int budgetId, int userId);

        /// <summary>
        /// Отримання бюджетів за типом
        /// </summary>
        Task<IEnumerable<BudgetDto>> GetBudgetsByTypeAsync(int familyId, BudgetType type, int userId);

        /// <summary>
        /// Отримання фінансового підсумку бюджету за період
        /// </summary>
        Task<BudgetSummaryDto> GetBudgetSummaryAsync(int budgetId, DateTime startDate, DateTime endDate, int userId);

        /// <summary>
        /// Перевірка, чи має користувач доступ до бюджету
        /// </summary>
        Task<bool> HasUserAccessToBudgetAsync(int budgetId, int userId);
    }
}
