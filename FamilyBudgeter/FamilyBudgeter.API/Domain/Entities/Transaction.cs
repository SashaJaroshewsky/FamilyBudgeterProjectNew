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
        public required Category Category { get; set; }

        public int BudgetId { get; set; }
        public required Budget Budget { get; set; }

        public int CreatedByUserId { get; set; }
        public required User CreatedByUser { get; set; }
    }
}
