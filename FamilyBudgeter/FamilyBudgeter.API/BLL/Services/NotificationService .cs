using FamilyBudgeter.API.BLL.DTOs.NotificationDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId);
            return notifications.Select(n => MapToDto(n));
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _unitOfWork.Notifications.GetUnreadAsync(userId);
            return notifications.Select(n => MapToDto(n));
        }

        public async Task<NotificationDto> GetNotificationByIdAsync(int notificationId, int userId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null)
            {
                throw new KeyNotFoundException("Сповіщення не знайдено");
            }

            // Перевірка, чи належить сповіщення користувачу
            if (notification.UserId != userId)
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього сповіщення");
            }

            return MapToDto(notification);
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto notificationDto)
        {
            var notification = new Notification
            {
                Title = notificationDto.Title,
                Message = notificationDto.Message,
                Type = notificationDto.Type,
                UserId = notificationDto.UserId,
                FamilyId = notificationDto.FamilyId,
                IsRead = false
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(notification);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null)
            {
                throw new KeyNotFoundException("Сповіщення не знайдено");
            }

            // Перевірка, чи належить сповіщення користувачу
            if (notification.UserId != userId)
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього сповіщення");
            }

            await _unitOfWork.Notifications.MarkAsReadAsync(notificationId);
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _unitOfWork.Notifications.GetUnreadAsync(userId);

            foreach (var notification in unreadNotifications)
            {
                await _unitOfWork.Notifications.MarkAsReadAsync(notification.Id);
            }

            return true;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null)
            {
                throw new KeyNotFoundException("Сповіщення не знайдено");
            }

            // Перевірка, чи належить сповіщення користувачу
            if (notification.UserId != userId)
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього сповіщення");
            }

            await _unitOfWork.Notifications.DeleteAsync(notificationId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByTypeAsync(int userId, NotificationType type)
        {
            var notifications = await _unitOfWork.Notifications.GetByTypeAsync(userId, type);
            return notifications.Select(n => MapToDto(n));
        }

        public async Task<IEnumerable<NotificationDto>> FilterNotificationsAsync(NotificationFilterDto filterDto, int userId)
        {
            var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId);

            if (filterDto.Type.HasValue)
            {
                notifications = notifications.Where(n => n.Type == filterDto.Type.Value);
            }

            if (filterDto.IsRead.HasValue)
            {
                notifications = notifications.Where(n => n.IsRead == filterDto.IsRead.Value);
            }

            if (filterDto.FamilyId.HasValue)
            {
                notifications = notifications.Where(n => n.FamilyId == filterDto.FamilyId.Value);
            }

            if (filterDto.StartDate.HasValue)
            {
                notifications = notifications.Where(n => n.CreatedAt >= filterDto.StartDate.Value);
            }

            if (filterDto.EndDate.HasValue)
            {
                notifications = notifications.Where(n => n.CreatedAt <= filterDto.EndDate.Value);
            }

            return notifications.OrderByDescending(n => n.CreatedAt).Select(n => MapToDto(n));
        }

        public async Task<NotificationDto> CreateLimitWarningNotificationAsync(int categoryId, int budgetId, int userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            var budget = await _unitOfWork.Budgets.GetByIdAsync(budgetId);

            if (category == null || budget == null)
            {
                throw new KeyNotFoundException("Категорія або бюджет не знайдено");
            }

            var notificationDto = new CreateNotificationDto
            {
                Title = "Попередження про ліміт",
                Message = $"Ліміт витрат для категорії '{category.Name}' наближається до перевищення",
                Type = NotificationType.LimitWarning,
                UserId = userId,
                FamilyId = budget.FamilyId
            };

            return await CreateNotificationAsync(notificationDto);
        }

        public async Task<NotificationDto> CreateRegularPaymentNotificationAsync(int paymentId, int userId)
        {
            var payment = await _unitOfWork.RegularPayments.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new KeyNotFoundException("Регулярний платіж не знайдено");
            }

            var budget = await _unitOfWork.Budgets.GetByIdAsync(payment.BudgetId);

            var notificationDto = new CreateNotificationDto
            {
                Title = "Нагадування про регулярний платіж",
                Message = $"Нагадування про регулярний платіж '{payment.Name}' на суму {payment.Amount} грн",
                Type = NotificationType.RegularPayment,
                UserId = userId,
                FamilyId = budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено")
            };

            return await CreateNotificationAsync(notificationDto);
        }

        public async Task<NotificationDto> CreateLargeExpenseNotificationAsync(int transactionId, int familyId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Транзакція не знайдена");
            }

            var creator = await _unitOfWork.Users.GetByIdAsync(transaction.CreatedByUserId);
            var category = await _unitOfWork.Categories.GetByIdAsync(transaction.CategoryId);

            // Відправляємо повідомлення всім членам сім'ї
            var familyMembers = await _unitOfWork.FamilyMembers.GetByFamilyIdAsync(familyId);
            var notifications = new List<NotificationDto>();

            foreach (var member in familyMembers)
            {
                // Не відправляємо повідомлення тому, хто створив транзакцію
                if (member.UserId == transaction.CreatedByUserId)
                {
                    continue;
                }

                var notificationDto = new CreateNotificationDto
                {
                    Title = "Значна витрата",
                    Message = $"{creator?.FirstName ?? throw new KeyNotFoundException("Бюджет не знайдено")} {creator.LastName} додав(ла) значну витрату на суму {transaction.Amount} грн в категорії '{category?.Name ?? throw new KeyNotFoundException("Бюджет не знайдено")}'",
                    Type = NotificationType.LargeExpense,
                    UserId = member.UserId,
                    FamilyId = familyId
                };

                var notification = await CreateNotificationAsync(notificationDto);
                notifications.Add(notification);
            }

            return notifications.FirstOrDefault() ?? throw new InvalidOperationException("No notifications were created.");
        }

        public async Task<NotificationDto> CreateGoalAchievementNotificationAsync(int goalId, int familyId)
        {
            var goal = await _unitOfWork.FinancialGoals.GetByIdAsync(goalId);
            if (goal == null)
            {
                throw new KeyNotFoundException("Фінансова ціль не знайдена");
            }

            // Відправляємо повідомлення всім членам сім'ї
            var familyMembers = await _unitOfWork.FamilyMembers.GetByFamilyIdAsync(familyId);
            var notifications = new List<NotificationDto>();

            foreach (var member in familyMembers)
            {
                var notificationDto = new CreateNotificationDto
                {
                    Title = "Фінансову ціль досягнуто!",
                    Message = $"Ваша сім'я досягла фінансової цілі '{goal.Name}'! Зібрано {goal.CurrentAmount} грн з {goal.TargetAmount} грн.",
                    Type = NotificationType.GoalAchievement,
                    UserId = member.UserId,
                    FamilyId = familyId
                };

                var notification = await CreateNotificationAsync(notificationDto);
                notifications.Add(notification);
            }

            return notifications.FirstOrDefault()
       ?? throw new InvalidOperationException("No notifications were created.");
        }

        #region Helper Methods

        private NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                Type = notification.Type,
                CreatedAt = notification.CreatedAt,
                UserId = notification.UserId,
                FamilyId = notification.FamilyId,
                FamilyName = notification.Family?.Name
            };
        }

        #endregion
    }
}
