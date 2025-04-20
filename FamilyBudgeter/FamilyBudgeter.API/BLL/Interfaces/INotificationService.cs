using FamilyBudgeter.API.BLL.DTOs.NotificationDTOs;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Отримання всіх сповіщень користувача
        /// </summary>
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);

        /// <summary>
        /// Отримання непрочитаних сповіщень користувача
        /// </summary>
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId);

        /// <summary>
        /// Отримання сповіщення за ідентифікатором
        /// </summary>
        Task<NotificationDto> GetNotificationByIdAsync(int notificationId, int userId);

        /// <summary>
        /// Створення нового сповіщення
        /// </summary>
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto notificationDto);

        /// <summary>
        /// Позначення сповіщення як прочитаного
        /// </summary>
        Task<bool> MarkAsReadAsync(int notificationId, int userId);

        /// <summary>
        /// Позначення всіх сповіщень користувача як прочитаних
        /// </summary>
        Task<bool> MarkAllAsReadAsync(int userId);

        /// <summary>
        /// Видалення сповіщення
        /// </summary>
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);

        /// <summary>
        /// Отримання сповіщень за типом
        /// </summary>
        Task<IEnumerable<NotificationDto>> GetNotificationsByTypeAsync(int userId, NotificationType type);

        /// <summary>
        /// Фільтрація сповіщень за різними параметрами
        /// </summary>
        Task<IEnumerable<NotificationDto>> FilterNotificationsAsync(NotificationFilterDto filterDto, int userId);

        /// <summary>
        /// Створення сповіщення про перевищення ліміту бюджету
        /// </summary>
        Task<NotificationDto> CreateLimitWarningNotificationAsync(int categoryId, int budgetId, int userId);

        /// <summary>
        /// Створення сповіщення про регулярний платіж
        /// </summary>
        Task<NotificationDto> CreateRegularPaymentNotificationAsync(int paymentId, int userId);

        /// <summary>
        /// Створення сповіщення про значну витрату
        /// </summary>
        Task<NotificationDto> CreateLargeExpenseNotificationAsync(int transactionId, int familyId);

        /// <summary>
        /// Створення сповіщення про досягнення фінансової цілі
        /// </summary>
        Task<NotificationDto> CreateGoalAchievementNotificationAsync(int goalId, int familyId);
    }
}
