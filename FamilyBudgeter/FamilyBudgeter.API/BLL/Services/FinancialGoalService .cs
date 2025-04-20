using FamilyBudgeter.API.BLL.DTOs.FinancialGoalDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Services
{
    public class FinancialGoalService : IFinancialGoalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFamilyService _familyService;
        private readonly IBudgetService _budgetService;
        private readonly INotificationService _notificationService;

        public FinancialGoalService(
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

        public async Task<IEnumerable<FinancialGoalDto>> GetBudgetGoalsAsync(int budgetId, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var goals = await _unitOfWork.FinancialGoals.GetByBudgetIdAsync(budgetId);
            return goals.Select(g => MapToDto(g));
        }

        public async Task<FinancialGoalDto> GetGoalByIdAsync(int goalId, int userId)
        {
            var goal = await _unitOfWork.FinancialGoals.GetByIdAsync(goalId);
            if (goal == null)
            {
                throw new KeyNotFoundException("Фінансова ціль не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(goal.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цієї фінансової цілі");
            }

            return MapToDto(goal);
        }

        public async Task<FinancialGoalDto> CreateGoalAsync(CreateFinancialGoalDto goalDto, int userId)
        {
            // Перевірка, чи належить користувач до сім'ї та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(goalDto.BudgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget.FamilyId, userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для створення фінансових цілей в цьому бюджеті");
            }

            // Перевірка, що дедлайн у майбутньому
            if (goalDto.Deadline <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Дедлайн має бути в майбутньому");
            }

            // Перевірка, що поточна сума не перевищує цільову
            if (goalDto.CurrentAmount > goalDto.TargetAmount)
            {
                throw new InvalidOperationException("Поточна сума не може перевищувати цільову");
            }

            var goal = new FinancialGoal
            {
                Name = goalDto.Name,
                Description = goalDto.Description,
                TargetAmount = goalDto.TargetAmount,
                CurrentAmount = goalDto.CurrentAmount,
                Deadline = goalDto.Deadline,
                Status = FinancialGoalStatus.InProgress,
                BudgetId = goalDto.BudgetId
            };

            await _unitOfWork.FinancialGoals.AddAsync(goal);
            await _unitOfWork.SaveChangesAsync();

            // Якщо ціль вже досягнута, оновлюємо статус
            if (goal.CurrentAmount >= goal.TargetAmount)
            {
                await CheckAndUpdateGoalStatusAsync(goal.Id, userId);
            }

            return MapToDto(goal);
        }

        public async Task<FinancialGoalDto> UpdateGoalAsync(int goalId, UpdateFinancialGoalDto goalDto, int userId)
        {
            var goal = await _unitOfWork.FinancialGoals.GetByIdAsync(goalId);
            if (goal == null)
            {
                throw new KeyNotFoundException("Фінансова ціль не знайдена");
            }

            // Перевірка, чи належить користувач до сім'ї та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(goal.BudgetId);
            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"), userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для оновлення фінансових цілей в цьому бюджеті");
            }

            // Перевірка, що дедлайн у майбутньому (якщо змінюється)
            if (goalDto.Deadline <= DateTime.UtcNow && goalDto.Deadline != goal.Deadline)
            {
                throw new InvalidOperationException("Дедлайн має бути в майбутньому");
            }

            // Перевірка, що поточна сума не перевищує цільову
            if (goalDto.CurrentAmount > goalDto.TargetAmount)
            {
                throw new InvalidOperationException("Поточна сума не може перевищувати цільову");
            }

            goal.Name = goalDto.Name;
            goal.Description = goalDto.Description;
            goal.TargetAmount = goalDto.TargetAmount;
            goal.CurrentAmount = goalDto.CurrentAmount;
            goal.Deadline = goalDto.Deadline;
            goal.Status = goalDto.Status;
            goal.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.FinancialGoals.Update(goal);
            await _unitOfWork.SaveChangesAsync();

            // Перевірка та оновлення статусу
            await CheckAndUpdateGoalStatusAsync(goalId, userId);

            return MapToDto(goal);
        }

        public async Task<bool> DeleteGoalAsync(int goalId, int userId)
        {
            var goal = await _unitOfWork.FinancialGoals.GetByIdAsync(goalId);
            if (goal == null)
            {
                throw new KeyNotFoundException("Фінансова ціль не знайдена");
            }

            // Перевірка, чи є користувач адміністратором сім'ї
            var budget = await _unitOfWork.Budgets.GetByIdAsync(goal.BudgetId);
            if (!await _familyService.IsUserFamilyAdminAsync(budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"), userId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може видаляти фінансові цілі");
            }

            await _unitOfWork.FinancialGoals.DeleteAsync(goalId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<FinancialGoalDto> UpdateGoalAmountAsync(int goalId, UpdateFinancialGoalAmountDto amountDto, int userId)
        {
            var goal = await _unitOfWork.FinancialGoals.GetByIdAsync(goalId);
            if (goal == null)
            {
                throw new KeyNotFoundException("Фінансова ціль не знайдена");
            }

            // Перевірка, чи належить користувач до сім'ї та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(goal.BudgetId);
            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"), userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для оновлення фінансових цілей в цьому бюджеті");
            }

            // Перевірка, що поточна сума не перевищує цільову
            if (amountDto.CurrentAmount > goal.TargetAmount)
            {
                throw new InvalidOperationException("Поточна сума не може перевищувати цільову");
            }

            goal.CurrentAmount = amountDto.CurrentAmount;
            goal.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.FinancialGoals.Update(goal);
            await _unitOfWork.SaveChangesAsync();

            // Перевірка та оновлення статусу
            await CheckAndUpdateGoalStatusAsync(goalId, userId);

            return MapToDto(goal);
        }

        public async Task<IEnumerable<FinancialGoalDto>> GetGoalsByStatusAsync(int budgetId, FinancialGoalStatus status, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var goals = await _unitOfWork.FinancialGoals.GetByStatusAsync(budgetId, status);
            return goals.Select(g => MapToDto(g));
        }

        public async Task<bool> CheckAndUpdateGoalStatusAsync(int goalId, int userId)
        {
            var goal = await _unitOfWork.FinancialGoals.GetByIdAsync(goalId);
            if (goal == null)
            {
                throw new KeyNotFoundException("Фінансова ціль не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(goal.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цієї фінансової цілі");
            }

            var statusChanged = false;

            // Перевірка, чи досягнута ціль
            if (goal.CurrentAmount >= goal.TargetAmount && goal.Status != FinancialGoalStatus.Achieved)
            {
                goal.Status = FinancialGoalStatus.Achieved;
                goal.UpdatedAt = DateTime.UtcNow;
                statusChanged = true;

                // Створення повідомлення про досягнення цілі
                var budget = await _unitOfWork.Budgets.GetByIdAsync(goal.BudgetId);
                await _notificationService.CreateGoalAchievementNotificationAsync(goalId, budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"));
            }
            // Перевірка, чи пройшов дедлайн без досягнення цілі
            else if (goal.Deadline < DateTime.UtcNow && goal.Status == FinancialGoalStatus.InProgress)
            {
                goal.Status = FinancialGoalStatus.Failed;
                goal.UpdatedAt = DateTime.UtcNow;
                statusChanged = true;
            }
            // Якщо була досягнута, але потім сума зменшилась нижче цільової
            else if (goal.CurrentAmount < goal.TargetAmount && goal.Status == FinancialGoalStatus.Achieved)
            {
                goal.Status = FinancialGoalStatus.InProgress;
                goal.UpdatedAt = DateTime.UtcNow;
                statusChanged = true;
            }

            if (statusChanged)
            {
                _unitOfWork.FinancialGoals.Update(goal);
                await _unitOfWork.SaveChangesAsync();
            }

            return statusChanged;
        }

        #region Helper Methods

        private FinancialGoalDto MapToDto(FinancialGoal goal)
        {
            return new FinancialGoalDto
            {
                Id = goal.Id,
                Name = goal.Name,
                Description = goal.Description,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                Deadline = goal.Deadline,
                Status = goal.Status,
                BudgetId = goal.BudgetId,
                PercentComplete = CalculatePercentComplete(goal.CurrentAmount, goal.TargetAmount),
                DaysRemaining = CalculateDaysRemaining(goal.Deadline)
            };
        }

        private decimal CalculatePercentComplete(decimal current, decimal target)
        {
            if (target == 0) return 0;
            return Math.Round((current / target) * 100, 2);
        }

        private int CalculateDaysRemaining(DateTime deadline)
        {
            var timeSpan = deadline - DateTime.UtcNow;
            return timeSpan.Days > 0 ? timeSpan.Days : 0;
        }

        #endregion
    }
}
