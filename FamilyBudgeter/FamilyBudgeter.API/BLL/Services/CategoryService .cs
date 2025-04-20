using FamilyBudgeter.API.BLL.DTOs.BudgetLimitDTOs;
using FamilyBudgeter.API.BLL.DTOs.CategoryDTOs;
using FamilyBudgeter.API.BLL.DTOs.TransactionDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFamilyService _familyService;
        private readonly IBudgetService _budgetService;

        public CategoryService(IUnitOfWork unitOfWork, IFamilyService familyService, IBudgetService budgetService)
        {
            _unitOfWork = unitOfWork;
            _familyService = familyService;
            _budgetService = budgetService;
        }

        public async Task<IEnumerable<CategoryDto>> GetBudgetCategoriesAsync(int budgetId, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            return categories.Select(c => MapToDto(c));
        }

        public async Task<CategoryDetailDto> GetCategoryByIdAsync(int categoryId, int userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Категорія не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету категорії
            if (!await _budgetService.HasUserAccessToBudgetAsync(category.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цієї категорії");
            }

            var categoryWithTransactions = await _unitOfWork.Categories.GetWithTransactionsAsync(categoryId);
            var budgetLimits = await _unitOfWork.BudgetLimits.GetByCategoryIdAsync(categoryId);

            var dto = MapToDetailDto(categoryWithTransactions!);
            dto.Limits = budgetLimits.Select(bl => new BudgetLimitDto
            {
                Id = bl.Id,
                Amount = bl.Amount,
                StartDate = bl.StartDate,
                EndDate = bl.EndDate,
                CategoryId = bl.CategoryId,
                CategoryName = category.Name,
                BudgetId = bl.BudgetId,
                CurrentSpent = 0, // Буде обчислено нижче
                PercentUsed = 0 // Буде обчислено нижче
            }).ToList();

            // Обчислення поточних витрат та відсотка використання для кожного ліміту
            var activeLimits = budgetLimits
                .Where(bl => bl.StartDate <= DateTime.UtcNow && bl.EndDate >= DateTime.UtcNow)
                .ToList();

            if (activeLimits.Any())
            {
                var currentLimit = activeLimits.First();
                var currentSpent = categoryWithTransactions!.Transactions
                    .Where(t => t.Date >= currentLimit.StartDate && t.Date <= currentLimit.EndDate)
                    .Sum(t => t.Amount);

                dto.CurrentAmount = currentSpent;
                dto.BudgetLimit = currentLimit.Amount;
                dto.PercentOfLimit = CalculatePercentage(currentSpent, currentLimit.Amount);

                // Оновлення значень для всіх лімітів
                foreach (var limit in dto.Limits)
                {
                    var spent = categoryWithTransactions!.Transactions
                        .Where(t => t.Date >= limit.StartDate && t.Date <= limit.EndDate)
                        .Sum(t => t.Amount);

                    limit.CurrentSpent = spent;
                    limit.PercentUsed = CalculatePercentage(spent, limit.Amount);
                }
            }

            return dto;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(categoryDto.BudgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget.FamilyId, userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для створення категорій в цьому бюджеті");
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                Icon = categoryDto.Icon,
                Type = categoryDto.Type,
                BudgetId = categoryDto.BudgetId
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int categoryId, UpdateCategoryDto categoryDto, int userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Категорія не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(category.BudgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget.FamilyId, userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для оновлення категорій в цьому бюджеті");
            }

            category.Name = categoryDto.Name;
            category.Icon = categoryDto.Icon;
            category.Type = categoryDto.Type;
            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId, int userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Категорія не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету та чи є він адміністратором
            var budget = await _unitOfWork.Budgets.GetByIdAsync(category.BudgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            if (!await _familyService.IsUserFamilyAdminAsync(budget.FamilyId, userId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може видаляти категорії");
            }

            // Перевірка, чи немає транзакцій, пов'язаних з цією категорією
            var transactions = await _unitOfWork.Transactions.GetByCategoryIdAsync(categoryId);
            if (transactions.Any())
            {
                throw new InvalidOperationException("Не можна видалити категорію, яка має пов'язані транзакції");
            }

            // Видалення пов'язаних лімітів бюджету
            var budgetLimits = await _unitOfWork.BudgetLimits.GetByCategoryIdAsync(categoryId);
            foreach (var limit in budgetLimits)
            {
                _unitOfWork.BudgetLimits.Delete(limit);
            }

            await _unitOfWork.Categories.DeleteAsync(categoryId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesByTypeAsync(int budgetId, CategoryType type, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByTypeAsync(budgetId, type);
            return categories.Select(c => MapToDto(c));
        }

        public async Task<IEnumerable<CategorySummaryDto>> GetCategorySummariesAsync(int budgetId, DateTime startDate, DateTime endDate, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var categories = await _unitOfWork.Categories.GetByBudgetIdAsync(budgetId);
            var summaries = new List<CategorySummaryDto>();

            foreach (var category in categories)
            {
                var transactions = await _unitOfWork.Transactions.GetByCategoryIdAsync(category.Id);
                var filteredTransactions = transactions
                    .Where(t => t.Date >= startDate && t.Date <= endDate)
                    .ToList();

                var budgetLimits = await _unitOfWork.BudgetLimits.GetByCategoryIdAsync(category.Id);
                var activeLimit = budgetLimits
                    .FirstOrDefault(bl => bl.StartDate <= DateTime.UtcNow && bl.EndDate >= DateTime.UtcNow);

                var summary = new CategorySummaryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Icon = category.Icon,
                    Type = category.Type,
                    Amount = filteredTransactions.Sum(t => t.Amount),
                    Limit = activeLimit?.Amount,
                    PercentOfLimit = activeLimit != null
                        ? CalculatePercentage(filteredTransactions.Sum(t => t.Amount), activeLimit.Amount)
                        : null,
                    TransactionsCount = filteredTransactions.Count
                };

                summaries.Add(summary);
            }

            return summaries;
        }

        #region Helper Methods

        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Icon = category.Icon,
                Type = category.Type,
                BudgetId = category.BudgetId
            };
        }

        private CategoryDetailDto MapToDetailDto(Category category)
        {
            return new CategoryDetailDto
            {
                Id = category.Id,
                Name = category.Name,
                Icon = category.Icon,
                Type = category.Type,
                BudgetId = category.BudgetId,
                Transactions = category.Transactions.Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Description = t.Description,
                    Date = t.Date,
                    ReceiptImageUrl = t.ReceiptImageUrl,
                    CategoryId = t.CategoryId,
                    CategoryName = category.Name,
                    BudgetId = t.BudgetId,
                    CreatedByUserId = t.CreatedByUserId,
                    CreatedByUserName = t.CreatedByUser != null
                        ? $"{t.CreatedByUser.FirstName} {t.CreatedByUser.LastName}"
                        : "Unknown User"
                }).ToList(),
                CurrentAmount = category.Transactions.Sum(t => t.Amount)
            };
        }

        private decimal CalculatePercentage(decimal value, decimal total)
        {
            if (total == 0) return 0;
            return Math.Round((value / total) * 100, 2);
        }

        #endregion
    }
}
