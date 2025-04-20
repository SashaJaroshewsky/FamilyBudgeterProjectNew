using FamilyBudgeter.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.BudgetDTOs
{
    public class CreateBudgetDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Required]
        public string Currency { get; set; } = "грн";

        [Required]
        public BudgetType Type { get; set; }

        [Required]
        public int FamilyId { get; set; }
    }
}
