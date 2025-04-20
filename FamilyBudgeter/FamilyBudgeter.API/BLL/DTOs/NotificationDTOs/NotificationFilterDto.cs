using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.DTOs.NotificationDTOs
{
    public class NotificationFilterDto
    {
        public NotificationType? Type { get; set; }
        public bool? IsRead { get; set; }
        public int? FamilyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
