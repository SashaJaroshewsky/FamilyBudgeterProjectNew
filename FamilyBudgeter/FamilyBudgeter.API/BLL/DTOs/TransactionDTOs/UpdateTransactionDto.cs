using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.TransactionDTOs
{
    public class UpdateTransactionDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сума повинна бути більше 0")]
        public decimal Amount { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(200)]
        public required string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string? ReceiptImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
