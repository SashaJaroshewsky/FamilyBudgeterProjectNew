using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.DTOs.BudgetDTOs
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Currency { get; set; } = "грн";
        public BudgetType Type { get; set; }
        public int FamilyId { get; set; }
        public required string FamilyName { get; set; }
    }
}
