using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.DTOs.NotificationDTOs
{
    public class CreateNotificationDto
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public NotificationType Type { get; set; }
        public int UserId { get; set; }
        public int? FamilyId { get; set; }
    }
}
