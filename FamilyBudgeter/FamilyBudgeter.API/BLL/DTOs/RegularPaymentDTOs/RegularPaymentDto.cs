using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.DTOs.RegularPaymentDTOs
{
    public class RegularPaymentDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PaymentFrequency Frequency { get; set; }
        public int DayOfMonth { get; set; }
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public int BudgetId { get; set; }
    }
}
