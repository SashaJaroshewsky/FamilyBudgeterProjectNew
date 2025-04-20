namespace FamilyBudgeter.API.BLL.DTOs.TransactionDTOs
{
    public class TransactionFilterDto
    {
        public int? BudgetId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? SearchTerm { get; set; }
    }
}
