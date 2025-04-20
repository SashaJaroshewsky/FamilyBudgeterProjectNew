using FamilyBudgeter.API.BLL.DTOs.BudgetDTOs;
using FamilyBudgeter.API.BLL.DTOs.CategoryDTOs;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface IAnalysisService
    {
        /// <summary>
        /// Отримання загальних підсумків бюджету за період
        /// </summary>
        Task<BudgetSummaryDto> GetBudgetSummaryAsync(int budgetId, DateTime startDate, DateTime endDate, int userId);

        /// <summary>
        /// Отримання підсумків за категоріями
        /// </summary>
        Task<IEnumerable<CategorySummaryDto>> GetCategorySummariesAsync(int budgetId, DateTime startDate, DateTime endDate, int userId);

        /// <summary>
        /// Отримання тенденцій витрат за період
        /// </summary>
        Task<Dictionary<DateTime, decimal>> GetExpenseTrendAsync(int budgetId, DateTime startDate, DateTime endDate, string groupBy, int userId);

        /// <summary>
        /// Отримання тенденцій доходів за період
        /// </summary>
        Task<Dictionary<DateTime, decimal>> GetIncomeTrendAsync(int budgetId, DateTime startDate, DateTime endDate, string groupBy, int userId);

        /// <summary>
        /// Порівняння фактичних витрат з запланованими лімітами
        /// </summary>
        Task<Dictionary<string, object>> CompareBudgetWithActualAsync(int budgetId, DateTime startDate, DateTime endDate, int userId);

        /// <summary>
        /// Прогнозування витрат на наступний період
        /// </summary>
        Task<Dictionary<string, decimal>> ForecastExpensesAsync(int budgetId, int months, int userId);

        /// <summary>
        /// Аналіз основних джерел витрат
        /// </summary>
        Task<Dictionary<string, decimal>> AnalyzeTopExpensesAsync(int budgetId, DateTime startDate, DateTime endDate, int limit, int userId);

        /// <summary>
        /// Аналіз основних джерел доходу
        /// </summary>
        Task<Dictionary<string, decimal>> AnalyzeTopIncomesAsync(int budgetId, DateTime startDate, DateTime endDate, int limit, int userId);

        /// <summary>
        /// Аналіз витрат за користувачами
        /// </summary>
        Task<Dictionary<string, decimal>> AnalyzeExpensesByUserAsync(int budgetId, DateTime startDate, DateTime endDate, int userId);

        /// <summary>
        /// Аналіз змін витрат за певний період
        /// </summary>
        Task<Dictionary<string, object>> AnalyzeExpenseChangesAsync(int budgetId, DateTime startDate, DateTime endDate, int userId);
    }
}
