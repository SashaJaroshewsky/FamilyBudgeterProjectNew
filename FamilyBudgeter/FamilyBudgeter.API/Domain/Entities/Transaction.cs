namespace FamilyBudgeter.API.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? ReceiptImageUrl { get; set; } // URL до фото чека

        // Зв'язки з іншими сутностями
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public int BudgetId { get; set; }
        public virtual Budget? Budget { get; set; }

        public int CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }
    }
}
