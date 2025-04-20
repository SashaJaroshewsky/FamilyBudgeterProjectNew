using System.Transactions;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.Domain.Entities
{
    public class Budget : BaseEntity
    {
        public required string Name { get; set; }
        public string Currency { get; set; } = "грн"; // Валюта за замовчуванням - гривня
        public BudgetType Type { get; set; } // Тип бюджету

        // Зв'язок із сім'єю
        public int FamilyId { get; set; }
        public virtual Family? Family { get; set; }

        // Навігаційні властивості
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<FinancialGoal> FinancialGoals { get; set; } = new List<FinancialGoal>();
        public virtual ICollection<BudgetLimit> BudgetLimits { get; set; } = new List<BudgetLimit>();
    }
}
