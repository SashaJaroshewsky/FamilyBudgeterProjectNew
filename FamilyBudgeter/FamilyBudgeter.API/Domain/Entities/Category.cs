using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.Domain.Entities
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public CategoryType Type { get; set; } // Тип категорії (дохід/витрата)

        public int BudgetId { get; set; }
        public required Budget Budget { get; set; }

        // Навігаційні властивості
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<BudgetLimit> BudgetLimits { get; set; } = new List<BudgetLimit>();
    }
}
