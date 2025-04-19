using System.Transactions;

namespace FamilyBudgeter.API.Domain.Entities
{
    public class User : BaseEntity
    {
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        // Навігаційні властивості
        public virtual ICollection<FamilyMember> FamilyMemberships { get; set; } = new List<FamilyMember>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
