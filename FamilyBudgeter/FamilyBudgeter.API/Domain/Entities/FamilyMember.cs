using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.Domain.Entities
{
    public class FamilyMember : BaseEntity
    {
        public int UserId { get; set; }
        public required User User { get; set; }

        public int FamilyId { get; set; }
        public required Family Family { get; set; }

        public FamilyRole Role { get; set; } // Роль у сім'ї
    }
}
