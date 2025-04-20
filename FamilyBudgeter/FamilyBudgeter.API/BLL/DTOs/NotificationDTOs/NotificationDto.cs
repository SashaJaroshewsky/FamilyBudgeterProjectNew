using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.DTOs.NotificationDTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public bool IsRead { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public int? FamilyId { get; set; }
        public string? FamilyName { get; set; }
    }
}
