using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.DTOs.CategoryDTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Icon { get; set; }
        public CategoryType Type { get; set; }
        public int BudgetId { get; set; }
    }
}
