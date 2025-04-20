using FamilyBudgeter.API.BLL.DTOs.FinancialGoalDTOs;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface IFinancialGoalService
    {
        /// <summary>
        /// Отримання всіх фінансових цілей бюджету
        /// </summary>
        Task<IEnumerable<FinancialGoalDto>> GetBudgetGoalsAsync(int budgetId, int userId);

        /// <summary>
        /// Отримання фінансової цілі за ідентифікатором
        /// </summary>
        Task<FinancialGoalDto> GetGoalByIdAsync(int goalId, int userId);

        /// <summary>
        /// Створення нової фінансової цілі
        /// </summary>
        Task<FinancialGoalDto> CreateGoalAsync(CreateFinancialGoalDto goalDto, int userId);

        /// <summary>
        /// Оновлення фінансової цілі
        /// </summary>
        Task<FinancialGoalDto> UpdateGoalAsync(int goalId, UpdateFinancialGoalDto goalDto, int userId);

        /// <summary>
        /// Видалення фінансової цілі
        /// </summary>
        Task<bool> DeleteGoalAsync(int goalId, int userId);

        /// <summary>
        /// Оновлення поточної суми фінансової цілі
        /// </summary>
        Task<FinancialGoalDto> UpdateGoalAmountAsync(int goalId, UpdateFinancialGoalAmountDto amountDto, int userId);

        /// <summary>
        /// Отримання фінансових цілей за статусом
        /// </summary>
        Task<IEnumerable<FinancialGoalDto>> GetGoalsByStatusAsync(int budgetId, FinancialGoalStatus status, int userId);

        /// <summary>
        /// Перевірка досягнення фінансової цілі та оновлення її статусу
        /// </summary>
        Task<bool> CheckAndUpdateGoalStatusAsync(int goalId, int userId);
    }
}
