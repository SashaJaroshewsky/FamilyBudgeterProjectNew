using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.FinancialGoalDTOs
{
    public class UpdateFinancialGoalAmountDto
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Поточна сума не може бути від'ємною")]
        public decimal CurrentAmount { get; set; }
    }
}
