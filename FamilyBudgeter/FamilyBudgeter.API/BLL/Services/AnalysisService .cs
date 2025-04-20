using FamilyBudgeter.API.BLL.DTOs.BudgetDTOs;
using FamilyBudgeter.API.BLL.DTOs.CategoryDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBudgetService _budgetService;

        public AnalysisService(IUnitOfWork unitOfWork, IBudgetService budgetService)
        {
            _unitOfWork = unitOfWork;
            _budgetService = budgetService;
        }

        public async Task<BudgetSummaryDto> GetBudgetSummaryAsync(int budgetId, DateTime startDate, DateTime endDate, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);
            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);

            var income = transactions
                .Where(t => categories.Any(c => c.Id == t.CategoryId && c.Type == CategoryType.Income))
                .Sum(t => t.Amount);

            var expense = transactions
                .Where(t => categories.Any(c => c.Id == t.CategoryId && c.Type == CategoryType.Expense))
                .Sum(t => t.Amount);

            return new BudgetSummaryDto
            {
                Id = budget.Id,
                Name = budget.Name,
                Currency = budget.Currency,
                TotalIncome = income,
                TotalExpense = expense,
                Balance = income - expense,
                StartDate = startDate,
                EndDate = endDate
            };
        }

        public async Task<IEnumerable<CategorySummaryDto>> GetCategorySummariesAsync(int budgetId, DateTime startDate, DateTime endDate, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);
            var budgetLimits = await _unitOfWork.BudgetLimits.GetByBudgetIdAsync(budgetId);

            var summaries = new List<CategorySummaryDto>();
            foreach (var category in categories)
            {
                var categoryTransactions = transactions.Where(t => t.CategoryId == category.Id).ToList();
                var amount = categoryTransactions.Sum(t => t.Amount);

                // Знаходимо активний ліміт для категорії
                var activeLimit = budgetLimits
                    .FirstOrDefault(l => l.CategoryId == category.Id &&
                                       l.StartDate <= endDate &&
                                       l.EndDate >= startDate);

                var summary = new CategorySummaryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Icon = category.Icon,
                    Type = category.Type,
                    Amount = amount,
                    Limit = activeLimit?.Amount,
                    PercentOfLimit = activeLimit != null ? CalculatePercentage(amount, activeLimit.Amount) : null,
                    TransactionsCount = categoryTransactions.Count
                };

                summaries.Add(summary);
            }

            return summaries;
        }

        public async Task<Dictionary<DateTime, decimal>> GetExpenseTrendAsync(int budgetId, DateTime startDate, DateTime endDate, string groupBy, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var expenseCategories = categories.Where(c => c.Type == CategoryType.Expense).Select(c => c.Id).ToList();
            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);
            var expenseTransactions = transactions.Where(t => expenseCategories.Contains(t.CategoryId));

            var trend = new Dictionary<DateTime, decimal>();

            switch (groupBy.ToLower())
            {
                case "day":
                    trend = expenseTransactions
                        .GroupBy(t => t.Date.Date)
                        .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
                    break;

                case "week":
                    trend = expenseTransactions
                        .GroupBy(t => t.Date.AddDays(-(int)t.Date.DayOfWeek).Date)
                        .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
                    break;

                case "month":
                    trend = expenseTransactions
                        .GroupBy(t => new DateTime(t.Date.Year, t.Date.Month, 1))
                        .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
                    break;

                case "year":
                    trend = expenseTransactions
                        .GroupBy(t => new DateTime(t.Date.Year, 1, 1))
                        .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
                    break;

                default:
                    throw new ArgumentException("Невірний параметр групування. Підтримується: day, week, month, year");
            }

            return trend;
        }

        public async Task<Dictionary<DateTime, decimal>> GetIncomeTrendAsync(int budgetId, DateTime startDate, DateTime endDate, string groupBy, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var incomeCategories = categories.Where(c => c.Type == CategoryType.Income).Select(c => c.Id).ToList();
            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);
            var incomeTransactions = transactions.Where(t => incomeCategories.Contains(t.CategoryId));

            var trend = new Dictionary<DateTime, decimal>();

            switch (groupBy.ToLower())
            {
                case "day":
                    trend = incomeTransactions
                        .GroupBy(t => t.Date.Date)
                        .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
                    break;

                case "week":
                    trend = incomeTransactions
                        .GroupBy(t => t.Date.AddDays(-(int)t.Date.DayOfWeek).Date)
                        .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
                    break;

                case "month":
                    trend = incomeTransactions
                        .GroupBy(t => new DateTime(t.Date.Year, t.Date.Month, 1))
                        .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
                    break;

                case "year":
                    trend = incomeTransactions
                        .GroupBy(t => new DateTime(t.Date.Year, 1, 1))
                        .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
                    break;

                default:
                    throw new ArgumentException("Невірний параметр групування. Підтримується: day, week, month, year");
            }

            return trend;
        }

        public async Task<Dictionary<string, object>> CompareBudgetWithActualAsync(int budgetId, DateTime startDate, DateTime endDate, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);
            var budgetLimits = await _unitOfWork.BudgetLimits.GetByBudgetIdAsync(budgetId);

            var result = new Dictionary<string, object>();
            var categoryComparisons = new List<object>();

            foreach (var category in categories.Where(c => c.Type == CategoryType.Expense))
            {
                var actual = transactions.Where(t => t.CategoryId == category.Id).Sum(t => t.Amount);
                var limits = budgetLimits.Where(l => l.CategoryId == category.Id &&
                                                    l.StartDate <= endDate &&
                                                    l.EndDate >= startDate);
                var budgeted = limits.Sum(l => l.Amount);

                categoryComparisons.Add(new
                {
                    CategoryName = category.Name,
                    Budgeted = budgeted,
                    Actual = actual,
                    Difference = budgeted - actual,
                    PercentageUsed = budgeted > 0 ? CalculatePercentage(actual, budgeted) : 0
                });
            }

            result["categories"] = categoryComparisons;
            result["totalBudgeted"] = categoryComparisons.Sum(c => ((dynamic)c).Budgeted);
            result["totalActual"] = categoryComparisons.Sum(c => ((dynamic)c).Actual);
            result["totalDifference"] = (decimal)result["totalBudgeted"] - (decimal)result["totalActual"];

            return result;
        }

        public async Task<Dictionary<string, decimal>> ForecastExpensesAsync(int budgetId, int months, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-3); // Аналізуємо останні 3 місяці для прогнозу

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var expenseCategories = categories.Where(c => c.Type == CategoryType.Expense).ToList();
            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);

            var forecast = new Dictionary<string, decimal>();

            foreach (var category in expenseCategories)
            {
                var categoryTransactions = transactions.Where(t => t.CategoryId == category.Id).ToList();
                if (!categoryTransactions.Any())
                {
                    forecast[category.Name] = 0;
                    continue;
                }

                // Обчислюємо середній місячний витрат
                var monthlyExpenses = categoryTransactions
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => g.Sum(t => t.Amount))
                    .ToList();

                var averageMonthly = monthlyExpenses.Any() ? monthlyExpenses.Average() : 0;
                forecast[category.Name] = averageMonthly * months;
            }

            forecast["Total"] = forecast.Values.Sum();
            return forecast;
        }

        public async Task<Dictionary<string, decimal>> AnalyzeTopExpensesAsync(int budgetId, DateTime startDate, DateTime endDate, int limit, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var expenseCategories = categories.Where(c => c.Type == CategoryType.Expense).ToList();
            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);

            var topExpenses = new Dictionary<string, decimal>();

            foreach (var category in expenseCategories)
            {
                var categoryTotal = transactions
                    .Where(t => t.CategoryId == category.Id)
                    .Sum(t => t.Amount);

                if (categoryTotal > 0)
                {
                    topExpenses[category.Name] = categoryTotal;
                }
            }

            // Повертаємо топ N категорій за витратами
            return topExpenses
                .OrderByDescending(e => e.Value)
                .Take(limit)
                .ToDictionary(e => e.Key, e => e.Value);
        }

        public async Task<Dictionary<string, decimal>> AnalyzeTopIncomesAsync(int budgetId, DateTime startDate, DateTime endDate, int limit, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var incomeCategories = categories.Where(c => c.Type == CategoryType.Income).ToList();
            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);

            var topIncomes = new Dictionary<string, decimal>();

            foreach (var category in incomeCategories)
            {
                var categoryTotal = transactions
                    .Where(t => t.CategoryId == category.Id)
                    .Sum(t => t.Amount);

                if (categoryTotal > 0)
                {
                    topIncomes[category.Name] = categoryTotal;
                }
            }

            // Повертаємо топ N категорій за доходами
            return topIncomes
                .OrderByDescending(i => i.Value)
                .Take(limit)
                .ToDictionary(i => i.Key, i => i.Value);
        }

        public async Task<Dictionary<string, decimal>> AnalyzeExpensesByUserAsync(int budgetId, DateTime startDate, DateTime endDate, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var expenseCategories = categories.Where(c => c.Type == CategoryType.Expense).Select(c => c.Id).ToList();
            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);
            var expenseTransactions = transactions.Where(t => expenseCategories.Contains(t.CategoryId));

            var expensesByUser = expenseTransactions
                .GroupBy(t => t.CreatedByUserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Total = g.Sum(t => t.Amount)
                })
                .ToList();

            var result = new Dictionary<string, decimal>();

            foreach (var expense in expensesByUser)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(expense.UserId);
                if (user != null)
                {
                    result[$"{user.FirstName} {user.LastName}"] = expense.Total;
                }
            }

            return result;
        }

        public async Task<Dictionary<string, object>> AnalyzeExpenseChangesAsync(int budgetId, DateTime startDate, DateTime endDate, int userId)
        {
            // Перевірка доступу
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var expenseCategories = categories.Where(c => c.Type == CategoryType.Expense).ToList();
            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);

            // Порівнюємо з попереднім періодом
            var periodLength = (endDate - startDate).Days;
            var previousEndDate = startDate.AddDays(-1);
            var previousStartDate = previousEndDate.AddDays(-periodLength);
            var previousTransactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, previousStartDate, previousEndDate);

            var result = new Dictionary<string, object>();
            var categoryChanges = new List<object>();

            foreach (var category in expenseCategories)
            {
                var currentPeriodTotal = transactions
                    .Where(t => t.CategoryId == category.Id)
                    .Sum(t => t.Amount);

                var previousPeriodTotal = previousTransactions
                    .Where(t => t.CategoryId == category.Id)
                    .Sum(t => t.Amount);

                var change = currentPeriodTotal - previousPeriodTotal;
                var changePercent = previousPeriodTotal > 0
                    ? CalculatePercentage(change, previousPeriodTotal)
                    : (currentPeriodTotal > 0 ? 100 : 0);

                categoryChanges.Add(new
                {
                    CategoryName = category.Name,
                    CurrentPeriod = currentPeriodTotal,
                    PreviousPeriod = previousPeriodTotal,
                    Change = change,
                    ChangePercent = changePercent
                });
            }

            result["categoryChanges"] = categoryChanges;
            result["totalCurrentPeriod"] = transactions
                .Where(t => expenseCategories.Any(c => c.Id == t.CategoryId))
                .Sum(t => t.Amount);
            result["totalPreviousPeriod"] = previousTransactions
                .Where(t => expenseCategories.Any(c => c.Id == t.CategoryId))
                .Sum(t => t.Amount);
            result["totalChange"] = (decimal)result["totalCurrentPeriod"] - (decimal)result["totalPreviousPeriod"];
            result["totalChangePercent"] = (decimal)result["totalPreviousPeriod"] > 0
                ? CalculatePercentage((decimal)result["totalChange"], (decimal)result["totalPreviousPeriod"])
                : ((decimal)result["totalCurrentPeriod"] > 0 ? 100 : 0);

            return result;
        }

        #region Helper Methods

        private decimal CalculatePercentage(decimal value, decimal total)
        {
            if (total == 0) return 0;
            return Math.Round((value / total) * 100, 2);
        }

        #endregion
    }
}
