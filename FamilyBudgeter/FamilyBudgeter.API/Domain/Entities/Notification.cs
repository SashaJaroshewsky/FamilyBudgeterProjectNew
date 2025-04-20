using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public bool IsRead { get; set; }
        public NotificationType Type { get; set; }

        public int UserId { get; set; }
        public virtual User? User { get; set; }

        public int? FamilyId { get; set; }
        public virtual Family? Family { get; set; }
    }
}
