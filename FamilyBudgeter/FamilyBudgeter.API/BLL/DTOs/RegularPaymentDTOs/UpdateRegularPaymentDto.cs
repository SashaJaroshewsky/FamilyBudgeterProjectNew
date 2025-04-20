using FamilyBudgeter.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.RegularPaymentDTOs
{
    public class UpdateRegularPaymentDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сума повинна бути більше 0")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public PaymentFrequency Frequency { get; set; }

        [Required]
        [Range(1, 31, ErrorMessage = "День місяця повинен бути від 1 до 31")]
        public int DayOfMonth { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
