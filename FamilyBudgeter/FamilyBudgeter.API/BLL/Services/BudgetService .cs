using FamilyBudgeter.API.BLL.DTOs.BudgetDTOs;
using FamilyBudgeter.API.BLL.DTOs.BudgetLimitDTOs;
using FamilyBudgeter.API.BLL.DTOs.CategoryDTOs;
using FamilyBudgeter.API.BLL.DTOs.FinancialGoalDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFamilyService _familyService;

        public BudgetService(IUnitOfWork unitOfWork, IFamilyService familyService)
        {
            _unitOfWork = unitOfWork;
            _familyService = familyService;
        }

        public async Task<IEnumerable<BudgetDto>> GetFamilyBudgetsAsync(int familyId, int userId)
        {
            // Перевірка, чи належить користувач до сім'ї
            if (!await _familyService.IsUserFamilyMemberAsync(familyId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цієї сім'ї");
            }

            var budgets = await _unitOfWork.Budgets.GetByFamilyIdAsync(familyId);
            var family = await _unitOfWork.Families.GetByIdAsync(familyId);

            return budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                Name = b.Name,
                Currency = b.Currency,
                Type = b.Type,
                FamilyId = b.FamilyId,
                FamilyName = family?.Name ?? string.Empty
            });
        }

        public async Task<BudgetDetailDto> GetBudgetByIdAsync(int budgetId, int userId)
        {
            var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            // Перевірка, чи має користувач доступ до бюджету
            if (!await HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var family = await _unitOfWork.Families.GetByIdAsync(budget.FamilyId);
            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var financialGoals = await _unitOfWork.FinancialGoals.GetByBudgetIdAsync(budgetId);
            var budgetLimits = await _unitOfWork.BudgetLimits.GetByBudgetIdAsync(budgetId);

            var dto = new BudgetDetailDto
            {
                Id = budget.Id,
                Name = budget.Name,
                Currency = budget.Currency,
                Type = budget.Type,
                FamilyId = budget.FamilyId,
                FamilyName = family?.Name ?? string.Empty,
                Categories = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon,
                    Type = c.Type,
                    BudgetId = c.BudgetId
                }).ToList(),
                FinancialGoals = financialGoals.Select(fg => new FinancialGoalDto
                {
                    Id = fg.Id,
                    Name = fg.Name,
                    Description = fg.Description,
                    TargetAmount = fg.TargetAmount,
                    CurrentAmount = fg.CurrentAmount,
                    Deadline = fg.Deadline,
                    Status = fg.Status,
                    BudgetId = fg.BudgetId,
                    PercentComplete = CalculatePercentComplete(fg.CurrentAmount, fg.TargetAmount),
                    DaysRemaining = (fg.Deadline - DateTime.UtcNow).Days
                }).ToList(),
                Limits = budgetLimits.Select(bl => new BudgetLimitDto
                {
                    Id = bl.Id,
                    Amount = bl.Amount,
                    StartDate = bl.StartDate,
                    EndDate = bl.EndDate,
                    CategoryId = bl.CategoryId,
                    CategoryName = categories.FirstOrDefault(c => c.Id == bl.CategoryId)?.Name ?? string.Empty,
                    BudgetId = bl.BudgetId,
                    CurrentSpent = 0, // Це потрібно обчислити, отримавши транзакції за категорією
                    PercentUsed = 0 // Це також потрібно обчислити
                }).ToList()
            };

            // Обчислення витрат для кожного ліміту
            foreach (var limit in dto.Limits)
            {
                var transactions = await _unitOfWork.Transactions.GetByCategoryIdAsync(limit.CategoryId);
                var currentSpent = transactions
                    .Where(t => t.Date >= limit.StartDate && t.Date <= limit.EndDate)
                    .Sum(t => t.Amount);

                limit.CurrentSpent = currentSpent;
                limit.PercentUsed = CalculatePercentComplete(currentSpent, limit.Amount);
            }

            return dto;
        }

        public async Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto budgetDto, int userId)
        {
            // Перевірка, чи належить користувач до сім'ї та чи є він адміністратором або повним учасником
            var userRole = await _familyService.GetUserRoleInFamilyAsync(budgetDto.FamilyId, userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для створення бюджету в цій сім'ї");
            }

            var budget = new Budget
            {
                Name = budgetDto.Name,
                Currency = budgetDto.Currency,
                Type = budgetDto.Type,
                FamilyId = budgetDto.FamilyId
            };

            await _unitOfWork.Budgets.AddAsync(budget);
            await _unitOfWork.SaveChangesAsync();

            var family = await _unitOfWork.Families.GetByIdAsync(budget.FamilyId);

            return new BudgetDto
            {
                Id = budget.Id,
                Name = budget.Name,
                Currency = budget.Currency,
                Type = budget.Type,
                FamilyId = budget.FamilyId,
                FamilyName = family?.Name ?? string.Empty
            };
        }

        public async Task<BudgetDto> UpdateBudgetAsync(int budgetId, UpdateBudgetDto budgetDto, int userId)
        {
            var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            // Перевірка, чи належить користувач до сім'ї та чи є він адміністратором або повним учасником
            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget.FamilyId, userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для оновлення цього бюджету");
            }

            budget.Name = budgetDto.Name;
            budget.Currency = budgetDto.Currency;
            budget.Type = budgetDto.Type;
            budget.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Budgets.Update(budget);
            await _unitOfWork.SaveChangesAsync();

            var family = await _unitOfWork.Families.GetByIdAsync(budget.FamilyId);

            return new BudgetDto
            {
                Id = budget.Id,
                Name = budget.Name,
                Currency = budget.Currency,
                Type = budget.Type,
                FamilyId = budget.FamilyId,
                FamilyName = family?.Name ?? string.Empty
            };
        }

        public async Task<bool> DeleteBudgetAsync(int budgetId, int userId)
        {
            var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            // Перевірка, чи є користувач адміністратором сім'ї
            if (!await _familyService.IsUserFamilyAdminAsync(budget.FamilyId, userId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може видаляти бюджети");
            }

            // Видалення всіх пов'язаних записів
            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            foreach (var category in categories)
            {
                _unitOfWork.Categories.Delete(category);
            }

            var transactions = await _unitOfWork.Transactions.GetByBudgetIdAsync(budgetId);
            foreach (var transaction in transactions)
            {
                _unitOfWork.Transactions.Delete(transaction);
            }

            var budgetLimits = await _unitOfWork.BudgetLimits.GetByBudgetIdAsync(budgetId);
            foreach (var limit in budgetLimits)
            {
                _unitOfWork.BudgetLimits.Delete(limit);
            }

            var financialGoals = await _unitOfWork.FinancialGoals.GetByBudgetIdAsync(budgetId);
            foreach (var goal in financialGoals)
            {
                _unitOfWork.FinancialGoals.Delete(goal);
            }

            var regularPayments = await _unitOfWork.RegularPayments.GetByBudgetIdAsync(budgetId);
            foreach (var payment in regularPayments)
            {
                _unitOfWork.RegularPayments.Delete(payment);
            }

            await _unitOfWork.Budgets.DeleteAsync(budgetId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BudgetDto>> GetBudgetsByTypeAsync(int familyId, BudgetType type, int userId)
        {
            // Перевірка, чи належить користувач до сім'ї
            if (!await _familyService.IsUserFamilyMemberAsync(familyId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цієї сім'ї");
            }

            var budgets = await _unitOfWork.Budgets.GetByTypeAsync(familyId, type);
            var family = await _unitOfWork.Families.GetByIdAsync(familyId);

            return budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                Name = b.Name,
                Currency = b.Currency,
                Type = b.Type,
                FamilyId = b.FamilyId,
                FamilyName = family?.Name ?? string.Empty
            });
        }

        public async Task<BudgetSummaryDto> GetBudgetSummaryAsync(int budgetId, DateTime startDate, DateTime endDate, int userId)
        {
            var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            // Перевірка, чи має користувач доступ до бюджету
            if (!await HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
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

        public async Task<bool> HasUserAccessToBudgetAsync(int budgetId, int userId)
        {
            var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetId);
            if (budget == null)
            {
                return false;
            }

            return await _familyService.IsUserFamilyMemberAsync(budget.FamilyId, userId);
        }

        #region Helper Methods

        private decimal CalculatePercentComplete(decimal current, decimal target)
        {
            if (target == 0) return 0;
            return Math.Round((current / target) * 100, 2);
        }

        #endregion
    }
}
