using FamilyBudgeter.API.BLL.DTOs.RegularPaymentDTOs;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface IRegularPaymentService
    {
        /// <summary>
        /// Отримання всіх регулярних платежів бюджету
        /// </summary>
        Task<IEnumerable<RegularPaymentDto>> GetBudgetRegularPaymentsAsync(int budgetId, int userId);

        /// <summary>
        /// Отримання регулярного платежу за ідентифікатором
        /// </summary>
        Task<RegularPaymentDto> GetRegularPaymentByIdAsync(int paymentId, int userId);

        /// <summary>
        /// Створення нового регулярного платежу
        /// </summary>
        Task<RegularPaymentDto> CreateRegularPaymentAsync(CreateRegularPaymentDto paymentDto, int userId);

        /// <summary>
        /// Оновлення регулярного платежу
        /// </summary>
        Task<RegularPaymentDto> UpdateRegularPaymentAsync(int paymentId, UpdateRegularPaymentDto paymentDto, int userId);

        /// <summary>
        /// Видалення регулярного платежу
        /// </summary>
        Task<bool> DeleteRegularPaymentAsync(int paymentId, int userId);

        /// <summary>
        /// Отримання регулярних платежів за категорією
        /// </summary>
        Task<IEnumerable<RegularPaymentDto>> GetRegularPaymentsByCategoryAsync(int categoryId, int userId);

        /// <summary>
        /// Отримання регулярних платежів за частотою
        /// </summary>
        Task<IEnumerable<RegularPaymentDto>> GetRegularPaymentsByFrequencyAsync(int budgetId, PaymentFrequency frequency, int userId);

        /// <summary>
        /// Отримання майбутніх регулярних платежів на певний період
        /// </summary>
        Task<IEnumerable<RegularPaymentDto>> GetUpcomingRegularPaymentsAsync(int budgetId, DateTime startDate, DateTime endDate, int userId);

        /// <summary>
        /// Генерація транзакцій для регулярних платежів на поточний день
        /// </summary>
        Task<int> ProcessDueRegularPaymentsAsync();
    }
}
