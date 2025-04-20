namespace FamilyBudgeter.API.Domain.Entities
{
    public class BudgetLimit : BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Зв'язки з іншими сутностями
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public int BudgetId { get; set; }
        public virtual Budget? Budget { get; set; }
    }
}
