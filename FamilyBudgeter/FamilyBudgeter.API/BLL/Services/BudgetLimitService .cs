using FamilyBudgeter.API.BLL.DTOs.BudgetLimitDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Services
{
    public class BudgetLimitService : IBudgetLimitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFamilyService _familyService;
        private readonly IBudgetService _budgetService;
        private readonly INotificationService _notificationService;

        public BudgetLimitService(
            IUnitOfWork unitOfWork,
            IFamilyService familyService,
            IBudgetService budgetService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _familyService = familyService;
            _budgetService = budgetService;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<BudgetLimitDto>> GetBudgetLimitsAsync(int budgetId, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var budgetLimits = await _unitOfWork.BudgetLimits.GetByBudgetIdAsync(budgetId);

            var limitDtos = new List<BudgetLimitDto>();
            foreach (var limit in budgetLimits)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(limit.CategoryId);
                var currentSpent = await CalculateCurrentSpentAsync(limit.CategoryId, limit.StartDate, limit.EndDate);

                limitDtos.Add(new BudgetLimitDto
                {
                    Id = limit.Id,
                    Amount = limit.Amount,
                    StartDate = limit.StartDate,
                    EndDate = limit.EndDate,
                    CategoryId = limit.CategoryId,
                    CategoryName = category?.Name ?? string.Empty,
                    BudgetId = limit.BudgetId,
                    CurrentSpent = currentSpent,
                    PercentUsed = CalculatePercentage(currentSpent, limit.Amount)
                });
            }

            return limitDtos;
        }

        public async Task<BudgetLimitDto> GetBudgetLimitByIdAsync(int limitId, int userId)
        {
            var budgetLimit = await _unitOfWork.BudgetLimits.GetByIdAsync(limitId);
            if (budgetLimit == null)
            {
                throw new KeyNotFoundException("Ліміт бюджету не знайдено");
            }

            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetLimit.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього ліміту");
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(budgetLimit.CategoryId);
            var currentSpent = await CalculateCurrentSpentAsync(budgetLimit.CategoryId, budgetLimit.StartDate, budgetLimit.EndDate);

            return new BudgetLimitDto
            {
                Id = budgetLimit.Id,
                Amount = budgetLimit.Amount,
                StartDate = budgetLimit.StartDate,
                EndDate = budgetLimit.EndDate,
                CategoryId = budgetLimit.CategoryId,
                CategoryName = category?.Name ?? string.Empty,
                BudgetId = budgetLimit.BudgetId,
                CurrentSpent = currentSpent,
                PercentUsed = CalculatePercentage(currentSpent, budgetLimit.Amount)
            };
        }

        public async Task<BudgetLimitDto> CreateBudgetLimitAsync(CreateBudgetLimitDto limitDto, int userId)
        {
            // Перевірка, чи належить користувач до сім'ї та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(limitDto.BudgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget.FamilyId, userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для створення лімітів в цьому бюджеті");
            }

            // Перевірка, чи належить категорія до цього бюджету
            var category = await _unitOfWork.Categories.GetByIdAsync(limitDto.CategoryId);
            if (category == null || category.BudgetId != limitDto.BudgetId)
            {
                throw new InvalidOperationException("Категорія не належить до цього бюджету");
            }

            // Перевірка на перетин дат з існуючими лімітами
            var existingLimits = await _unitOfWork.BudgetLimits.GetByCategoryIdAsync(limitDto.CategoryId);
            if (existingLimits.Any(l =>
                (limitDto.StartDate >= l.StartDate && limitDto.StartDate <= l.EndDate) ||
                (limitDto.EndDate >= l.StartDate && limitDto.EndDate <= l.EndDate) ||
                (limitDto.StartDate <= l.StartDate && limitDto.EndDate >= l.EndDate)))
            {
                throw new InvalidOperationException("Період ліміту перетинається з існуючим лімітом");
            }

            var budgetLimit = new BudgetLimit
            {
                Amount = limitDto.Amount,
                StartDate = limitDto.StartDate,
                EndDate = limitDto.EndDate,
                CategoryId = limitDto.CategoryId,
                BudgetId = limitDto.BudgetId
            };

            await _unitOfWork.BudgetLimits.AddAsync(budgetLimit);
            await _unitOfWork.SaveChangesAsync();

            var currentSpent = await CalculateCurrentSpentAsync(budgetLimit.CategoryId, budgetLimit.StartDate, budgetLimit.EndDate);

            return new BudgetLimitDto
            {
                Id = budgetLimit.Id,
                Amount = budgetLimit.Amount,
                StartDate = budgetLimit.StartDate,
                EndDate = budgetLimit.EndDate,
                CategoryId = budgetLimit.CategoryId,
                CategoryName = category.Name,
                BudgetId = budgetLimit.BudgetId,
                CurrentSpent = currentSpent,
                PercentUsed = CalculatePercentage(currentSpent, budgetLimit.Amount)
            };
        }

        public async Task<BudgetLimitDto> UpdateBudgetLimitAsync(int limitId, UpdateBudgetLimitDto limitDto, int userId)
        {
            var budgetLimit = await _unitOfWork.BudgetLimits.GetByIdAsync(limitId);
            if (budgetLimit == null)
            {
                throw new KeyNotFoundException("Ліміт бюджету не знайдено");
            }

            // Перевірка, чи належить користувач до сім'ї та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetLimit.BudgetId);
            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"), userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для оновлення лімітів в цьому бюджеті");
            }

            // Перевірка на перетин дат з існуючими лімітами (крім поточного)
            var existingLimits = await _unitOfWork.BudgetLimits.GetByCategoryIdAsync(budgetLimit.CategoryId);
            if (existingLimits.Any(l =>
                l.Id != limitId &&
                ((limitDto.StartDate >= l.StartDate && limitDto.StartDate <= l.EndDate) ||
                (limitDto.EndDate >= l.StartDate && limitDto.EndDate <= l.EndDate) ||
                (limitDto.StartDate <= l.StartDate && limitDto.EndDate >= l.EndDate))))
            {
                throw new InvalidOperationException("Період ліміту перетинається з існуючим лімітом");
            }

            budgetLimit.Amount = limitDto.Amount;
            budgetLimit.StartDate = limitDto.StartDate;
            budgetLimit.EndDate = limitDto.EndDate;
            budgetLimit.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.BudgetLimits.Update(budgetLimit);
            await _unitOfWork.SaveChangesAsync();

            var category = await _unitOfWork.Categories.GetByIdAsync(budgetLimit.CategoryId);
            var currentSpent = await CalculateCurrentSpentAsync(budgetLimit.CategoryId, budgetLimit.StartDate, budgetLimit.EndDate);

            return new BudgetLimitDto
            {
                Id = budgetLimit.Id,
                Amount = budgetLimit.Amount,
                StartDate = budgetLimit.StartDate,
                EndDate = budgetLimit.EndDate,
                CategoryId = budgetLimit.CategoryId,
                CategoryName = category?.Name ?? string.Empty,
                BudgetId = budgetLimit.BudgetId,
                CurrentSpent = currentSpent,
                PercentUsed = CalculatePercentage(currentSpent, budgetLimit.Amount)
            };
        }

        public async Task<bool> DeleteBudgetLimitAsync(int limitId, int userId)
        {
            var budgetLimit = await _unitOfWork.BudgetLimits.GetByIdAsync(limitId);
            if (budgetLimit == null)
            {
                throw new KeyNotFoundException("Ліміт бюджету не знайдено");
            }

            // Перевірка, чи є користувач адміністратором сім'ї
            var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetLimit.BudgetId);
            if (!await _familyService.IsUserFamilyAdminAsync(budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"), userId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може видаляти ліміти");
            }

            await _unitOfWork.BudgetLimits.DeleteAsync(limitId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BudgetLimitDto>> GetActiveBudgetLimitsAsync(int budgetId, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var activeLimits = await _unitOfWork.BudgetLimits.GetActiveAsync(budgetId, DateTime.UtcNow);

            var limitDtos = new List<BudgetLimitDto>();
            foreach (var limit in activeLimits)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(limit.CategoryId);
                var currentSpent = await CalculateCurrentSpentAsync(limit.CategoryId, limit.StartDate, limit.EndDate);

                limitDtos.Add(new BudgetLimitDto
                {
                    Id = limit.Id,
                    Amount = limit.Amount,
                    StartDate = limit.StartDate,
                    EndDate = limit.EndDate,
                    CategoryId = limit.CategoryId,
                    CategoryName = category?.Name ?? string.Empty,
                    BudgetId = limit.BudgetId,
                    CurrentSpent = currentSpent,
                    PercentUsed = CalculatePercentage(currentSpent, limit.Amount)
                });
            }

            return limitDtos;
        }

        public async Task<IEnumerable<BudgetLimitDto>> GetLimitsByCategoryAsync(int categoryId, int userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Категорія не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(category.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до лімітів цієї категорії");
            }

            var limits = await _unitOfWork.BudgetLimits.GetByCategoryIdAsync(categoryId);

            var limitDtos = new List<BudgetLimitDto>();
            foreach (var limit in limits)
            {
                var currentSpent = await CalculateCurrentSpentAsync(limit.CategoryId, limit.StartDate, limit.EndDate);

                limitDtos.Add(new BudgetLimitDto
                {
                    Id = limit.Id,
                    Amount = limit.Amount,
                    StartDate = limit.StartDate,
                    EndDate = limit.EndDate,
                    CategoryId = limit.CategoryId,
                    CategoryName = category.Name,
                    BudgetId = limit.BudgetId,
                    CurrentSpent = currentSpent,
                    PercentUsed = CalculatePercentage(currentSpent, limit.Amount)
                });
            }

            return limitDtos;
        }

        public async Task<bool> IsLimitExceededAsync(int categoryId, int userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Категорія не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(category.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до лімітів цієї категорії");
            }

            // Отримання активних лімітів для категорії
            var activeLimits = await _unitOfWork.BudgetLimits.GetActiveAsync(category.BudgetId, DateTime.UtcNow);
            var categoryLimit = activeLimits.FirstOrDefault(l => l.CategoryId == categoryId);

            if (categoryLimit == null)
            {
                return false; // Немає ліміту, значить не перевищено
            }

            var currentSpent = await CalculateCurrentSpentAsync(categoryId, categoryLimit.StartDate, categoryLimit.EndDate);
            return currentSpent > categoryLimit.Amount;
        }

        public async Task<IEnumerable<BudgetLimitDto>> GetLimitStatusesAsync(int budgetId, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var activeLimits = await _unitOfWork.BudgetLimits.GetActiveAsync(budgetId, DateTime.UtcNow);

            var limitStatuses = new List<BudgetLimitDto>();
            foreach (var limit in activeLimits)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(limit.CategoryId);
                var currentSpent = await CalculateCurrentSpentAsync(limit.CategoryId, limit.StartDate, limit.EndDate);
                var percentUsed = CalculatePercentage(currentSpent, limit.Amount);

                // Створення повідомлення, якщо наближаємось до ліміту
                if (percentUsed >= 80 && percentUsed < 100)
                {
                    await _notificationService.CreateLimitWarningNotificationAsync(limit.CategoryId, limit.BudgetId, userId);
                }
                else if (percentUsed >= 100)
                {
                    // Можна створити більш серйозне повідомлення про перевищення ліміту
                    await _notificationService.CreateLimitWarningNotificationAsync(limit.CategoryId, limit.BudgetId, userId);
                }

                limitStatuses.Add(new BudgetLimitDto
                {
                    Id = limit.Id,
                    Amount = limit.Amount,
                    StartDate = limit.StartDate,
                    EndDate = limit.EndDate,
                    CategoryId = limit.CategoryId,
                    CategoryName = category?.Name ?? string.Empty,
                    BudgetId = limit.BudgetId,
                    CurrentSpent = currentSpent,
                    PercentUsed = percentUsed
                });
            }

            return limitStatuses;
        }

        #region Helper Methods

        private async Task<decimal> CalculateCurrentSpentAsync(int categoryId, DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Transactions.GetSumByCategoryAsync(categoryId, startDate, endDate);
        }

        private decimal CalculatePercentage(decimal value, decimal total)
        {
            if (total == 0) return 0;
            return Math.Round((value / total) * 100, 2);
        }

        #endregion
    }
}
