namespace FamilyBudgeter.API.BLL.DTOs.TransactionDTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public  string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? ReceiptImageUrl { get; set; }
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public int BudgetId { get; set; }
        public int CreatedByUserId { get; set; }
        public required string CreatedByUserName { get; set; }
    }
}
