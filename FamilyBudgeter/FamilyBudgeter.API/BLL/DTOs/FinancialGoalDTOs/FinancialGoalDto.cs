using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.DTOs.FinancialGoalDTOs
{
    public class FinancialGoalDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime Deadline { get; set; }
        public FinancialGoalStatus Status { get; set; }
        public int BudgetId { get; set; }
        public decimal PercentComplete { get; set; }
        public int DaysRemaining { get; set; }
    }
}
