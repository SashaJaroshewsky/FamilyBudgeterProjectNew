using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.Domain.Entities
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public string? Icon { get; set; } // Іконка для категорії (nullable)
        public CategoryType Type { get; set; } // Тип категорії (дохід/витрата)

        public int BudgetId { get; set; }
        public virtual Budget? Budget { get; set; }

        // Навігаційні властивості
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<BudgetLimit> BudgetLimits { get; set; } = new List<BudgetLimit>();
    }
}
