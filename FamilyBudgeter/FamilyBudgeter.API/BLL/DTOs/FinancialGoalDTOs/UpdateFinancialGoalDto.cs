using FamilyBudgeter.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.FinancialGoalDTOs
{
    public class UpdateFinancialGoalDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цільова сума повинна бути більше 0")]
        public decimal TargetAmount { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Поточна сума не може бути від'ємною")]
        public decimal CurrentAmount { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public FinancialGoalStatus Status { get; set; }
    }
}
