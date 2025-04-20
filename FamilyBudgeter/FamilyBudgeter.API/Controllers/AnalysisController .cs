using FamilyBudgeter.API.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    [Authorize]
    public class AnalysisController : BaseApiController
    {
        private readonly IAnalysisService _analysisService;

        public AnalysisController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        [HttpGet("budget/{budgetId}/summary")]
        public async Task<IActionResult> GetBudgetSummary(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var summary = await _analysisService.GetBudgetSummaryAsync(budgetId, startDate, endDate, userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/category-summaries")]
        public async Task<IActionResult> GetCategorySummaries(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var summaries = await _analysisService.GetCategorySummariesAsync(budgetId, startDate, endDate, userId);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/expense-trend")]
        public async Task<IActionResult> GetExpenseTrend(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string groupBy = "month")
        {
            try
            {
                var userId = GetCurrentUserId();
                var trend = await _analysisService.GetExpenseTrendAsync(budgetId, startDate, endDate, groupBy, userId);
                return Ok(trend);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/income-trend")]
        public async Task<IActionResult> GetIncomeTrend(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string groupBy = "month")
        {
            try
            {
                var userId = GetCurrentUserId();
                var trend = await _analysisService.GetIncomeTrendAsync(budgetId, startDate, endDate, groupBy, userId);
                return Ok(trend);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/compare-with-actual")]
        public async Task<IActionResult> CompareBudgetWithActual(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var comparison = await _analysisService.CompareBudgetWithActualAsync(budgetId, startDate, endDate, userId);
                return Ok(comparison);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/forecast")]
        public async Task<IActionResult> ForecastExpenses(int budgetId, [FromQuery] int months = 3)
        {
            try
            {
                var userId = GetCurrentUserId();
                var forecast = await _analysisService.ForecastExpensesAsync(budgetId, months, userId);
                return Ok(forecast);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/top-expenses")]
        public async Task<IActionResult> AnalyzeTopExpenses(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int limit = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                var topExpenses = await _analysisService.AnalyzeTopExpensesAsync(budgetId, startDate, endDate, limit, userId);
                return Ok(topExpenses);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/top-incomes")]
        public async Task<IActionResult> AnalyzeTopIncomes(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int limit = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                var topIncomes = await _analysisService.AnalyzeTopIncomesAsync(budgetId, startDate, endDate, limit, userId);
                return Ok(topIncomes);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/expenses-by-user")]
        public async Task<IActionResult> AnalyzeExpensesByUser(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var expensesByUser = await _analysisService.AnalyzeExpensesByUserAsync(budgetId, startDate, endDate, userId);
                return Ok(expensesByUser);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("budget/{budgetId}/expense-changes")]
        public async Task<IActionResult> AnalyzeExpenseChanges(int budgetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var changes = await _analysisService.AnalyzeExpenseChangesAsync(budgetId, startDate, endDate, userId);
                return Ok(changes);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
