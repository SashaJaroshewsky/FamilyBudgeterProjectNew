using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.Domain.Entities
{
    public class RegularPayment : BaseEntity
    {
        public required string Name { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Опціонально - дата закінчення
        public PaymentFrequency Frequency { get; set; }
        public int DayOfMonth { get; set; } // День місяця для щомісячних платежів

        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public int BudgetId { get; set; }
        public virtual Budget? Budget { get; set; }
    }
}
