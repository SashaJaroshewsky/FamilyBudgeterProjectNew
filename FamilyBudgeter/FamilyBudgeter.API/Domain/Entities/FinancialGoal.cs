using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.Domain.Entities
{
    public class FinancialGoal : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime Deadline { get; set; }
        public FinancialGoalStatus Status { get; set; } = FinancialGoalStatus.InProgress;

        public int BudgetId { get; set; }
        public required Budget Budget { get; set; }
    }
}
