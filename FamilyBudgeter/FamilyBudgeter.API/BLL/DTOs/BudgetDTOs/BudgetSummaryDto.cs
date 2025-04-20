namespace FamilyBudgeter.API.BLL.DTOs.BudgetDTOs
{
    public class BudgetSummaryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Currency { get; set; } = "грн";
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
