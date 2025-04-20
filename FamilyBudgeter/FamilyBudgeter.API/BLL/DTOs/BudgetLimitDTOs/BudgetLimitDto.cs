namespace FamilyBudgeter.API.BLL.DTOs.BudgetLimitDTOs
{
    public class BudgetLimitDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public int BudgetId { get; set; }
        public decimal CurrentSpent { get; set; }
        public decimal PercentUsed { get; set; }
    }
}
