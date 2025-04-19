namespace FamilyBudgeter.API.Domain.Entities
{
    public class Family : BaseEntity
    {
        public required string Name { get; set; }
        public required string JoinCode { get; set; } // Код для приєднання до сім'ї

        // Навігаційні властивості
        public virtual ICollection<FamilyMember> Members { get; set; } = new List<FamilyMember>();
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
