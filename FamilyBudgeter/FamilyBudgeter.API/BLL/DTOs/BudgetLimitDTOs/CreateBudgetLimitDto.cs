using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.BudgetLimitDTOs
{
    public class CreateBudgetLimitDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сума ліміту повинна бути більше 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BudgetId { get; set; }
    }
}
