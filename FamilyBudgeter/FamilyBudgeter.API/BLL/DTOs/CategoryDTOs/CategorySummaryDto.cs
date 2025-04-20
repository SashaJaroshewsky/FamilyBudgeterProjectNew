using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.DTOs.CategoryDTOs
{
    public class CategorySummaryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Icon { get; set; }
        public CategoryType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal? Limit { get; set; }
        public decimal? PercentOfLimit { get; set; }
        public int TransactionsCount { get; set; }
    }
}
