using FamilyBudgeter.API.BLL.DTOs.TransactionDTOs;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface ITransactionService
    {
        /// <summary>
        /// Отримання всіх транзакцій бюджету
        /// </summary>
        Task<IEnumerable<TransactionDto>> GetBudgetTransactionsAsync(int budgetId, int userId);

        /// <summary>
        /// Отримання транзакції за ідентифікатором
        /// </summary>
        Task<TransactionDto> GetTransactionByIdAsync(int transactionId, int userId);

        /// <summary>
        /// Створення нової транзакції
        /// </summary>
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto transactionDto, int userId);

        /// <summary>
        /// Оновлення транзакції
        /// </summary>
        Task<TransactionDto> UpdateTransactionAsync(int transactionId, UpdateTransactionDto transactionDto, int userId);

        /// <summary>
        /// Видалення транзакції
        /// </summary>
        Task<bool> DeleteTransactionAsync(int transactionId, int userId);

        /// <summary>
        /// Отримання транзакцій за категорією
        /// </summary>
        Task<IEnumerable<TransactionDto>> GetTransactionsByCategoryAsync(int categoryId, int userId);

        /// <summary>
        /// Отримання транзакцій за діапазоном дат
        /// </summary>
        Task<IEnumerable<TransactionDto>> GetTransactionsByDateRangeAsync(int budgetId, DateTime startDate, DateTime endDate, int userId);

        /// <summary>
        /// Отримання транзакцій, створених користувачем
        /// </summary>
        Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId);

        /// <summary>
        /// Фільтрація транзакцій за різними параметрами
        /// </summary>
        Task<IEnumerable<TransactionDto>> FilterTransactionsAsync(TransactionFilterDto filterDto, int userId);

        /// <summary>
        /// Завантаження зображення чеку
        /// </summary>
        Task<string> UploadReceiptImageAsync(int transactionId, Stream imageStream, string fileName, int userId);
    }
}
