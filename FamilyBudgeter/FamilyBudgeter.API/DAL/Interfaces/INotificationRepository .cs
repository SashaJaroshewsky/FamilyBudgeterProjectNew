using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        /// <summary>
        /// Отримати всі сповіщення користувача
        /// </summary>
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Отримати непрочитані сповіщення користувача
        /// </summary>
        Task<IEnumerable<Notification>> GetUnreadAsync(int userId);

        /// <summary>
        /// Отримати сповіщення за типом
        /// </summary>
        Task<IEnumerable<Notification>> GetByTypeAsync(int userId, NotificationType type);

        /// <summary>
        /// Позначити сповіщення як прочитане
        /// </summary>
        Task MarkAsReadAsync(int notificationId);
    }
}
