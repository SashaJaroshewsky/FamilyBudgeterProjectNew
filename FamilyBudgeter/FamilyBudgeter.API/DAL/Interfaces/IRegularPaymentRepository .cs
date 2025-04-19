using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface IRegularPaymentRepository : IGenericRepository<RegularPayment>
    {
        /// <summary>
        /// Отримати всі регулярні платежі бюджету
        /// </summary>
        Task<IEnumerable<RegularPayment>> GetByBudgetIdAsync(int budgetId);

        /// <summary>
        /// Отримати регулярні платежі за частотою
        /// </summary>
        Task<IEnumerable<RegularPayment>> GetByFrequencyAsync(int budgetId, PaymentFrequency frequency);

        /// <summary>
        /// Отримати всі активні регулярні платежі на вказану дату
        /// </summary>
        Task<IEnumerable<RegularPayment>> GetActiveAsync(int budgetId, DateTime date);
    }
}
