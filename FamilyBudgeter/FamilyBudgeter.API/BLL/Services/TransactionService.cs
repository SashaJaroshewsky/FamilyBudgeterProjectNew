using FamilyBudgeter.API.BLL.DTOs.TransactionDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFamilyService _familyService;
        private readonly IBudgetService _budgetService;
        private readonly INotificationService _notificationService;

        public TransactionService(
            IUnitOfWork unitOfWork,
            IFamilyService familyService,
            IBudgetService budgetService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _familyService = familyService;
            _budgetService = budgetService;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<TransactionDto>> GetBudgetTransactionsAsync(int budgetId, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var transactions = await _unitOfWork.Transactions.GetByBudgetIdAsync(budgetId);
            return transactions.Select(t => MapToDto(t));
        }

        public async Task<TransactionDto> GetTransactionByIdAsync(int transactionId, int userId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Транзакція не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету транзакції
            if (!await _budgetService.HasUserAccessToBudgetAsync(transaction.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цієї транзакції");
            }

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto transactionDto, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(transactionDto.BudgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget.FamilyId, userId);
            if (userRole == null) // Перевірка чи користувач належить до сім'ї
            {
                throw new UnauthorizedAccessException("У вас немає прав для створення транзакцій в цьому бюджеті");
            }

            // Обмежені учасники можуть додавати лише власні витрати
            var category = await _unitOfWork.Categories.GetByIdAsync(transactionDto.CategoryId);
            if (userRole == FamilyRole.LimitedMember && category?.Type == CategoryType.Income)
            {
                throw new UnauthorizedAccessException("Обмежені учасники можуть додавати лише витрати");
            }

            var transaction = new Transaction
            {
                Amount = transactionDto.Amount,
                Description = transactionDto.Description,
                Date = transactionDto.Date,
                ReceiptImageUrl = transactionDto.ReceiptImageUrl,
                CategoryId = transactionDto.CategoryId,
                BudgetId = transactionDto.BudgetId,
                CreatedByUserId = userId
            };

            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            // Перевірка на перевищення ліміту
            await CheckBudgetLimitAndNotifyAsync(transaction, userId);

            // Створення повідомлення про значну витрату
            if (category?.Type == CategoryType.Expense && transaction.Amount > 1000) // Можна налаштувати поріг
            {
                await _notificationService.CreateLargeExpenseNotificationAsync(transaction.Id, budget.FamilyId);
            }

            return MapToDto(transaction);
        }

        public async Task<TransactionDto> UpdateTransactionAsync(int transactionId, UpdateTransactionDto transactionDto, int userId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Транзакція не знайдена");
            }

            // Перевірка, чи має користувач право оновлювати транзакцію
            var budget = await _unitOfWork.Budgets.GetByIdAsync(transaction.BudgetId);
            
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget.FamilyId, userId);
            // Адміністратор може редагувати будь-які транзакції
            // Повний учасник може редагувати власні транзакції
            // Обмежений учасник може редагувати тільки власні транзакції-витрати
            if (userRole == null ||
                (userRole == FamilyRole.FullMember && transaction.CreatedByUserId != userId) ||
                (userRole == FamilyRole.LimitedMember &&
                    (transaction.CreatedByUserId != userId ||
                     (await _unitOfWork.Categories.GetByIdAsync(transactionDto.CategoryId))?.Type == CategoryType.Income)))
            {
                throw new UnauthorizedAccessException("У вас немає прав для оновлення цієї транзакції");
            }

            transaction.Amount = transactionDto.Amount;
            transaction.Description = transactionDto.Description;
            transaction.Date = transactionDto.Date;
            transaction.ReceiptImageUrl = transactionDto.ReceiptImageUrl;
            transaction.CategoryId = transactionDto.CategoryId;
            transaction.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Transactions.Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            // Перевірка на перевищення ліміту після оновлення
            await CheckBudgetLimitAndNotifyAsync(transaction, userId);

            return MapToDto(transaction);
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId, int userId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Транзакція не знайдена");
            }

            // Перевірка, чи має користувач право видаляти транзакцію
            var budget = await _unitOfWork.Budgets.GetByIdAsync(transaction.BudgetId);
            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"), userId);

            if (userRole == null ||
                (userRole == FamilyRole.FullMember && transaction.CreatedByUserId != userId) ||
                (userRole == FamilyRole.LimitedMember))
            {
                throw new UnauthorizedAccessException("У вас немає прав для видалення цієї транзакції");
            }

            await _unitOfWork.Transactions.DeleteAsync(transactionId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByCategoryAsync(int categoryId, int userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Категорія не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету категорії
            if (!await _budgetService.HasUserAccessToBudgetAsync(category.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до транзакцій цієї категорії");
            }

            var transactions = await _unitOfWork.Transactions.GetByCategoryIdAsync(categoryId);
            return transactions.Select(t => MapToDto(t));
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByDateRangeAsync(int budgetId, DateTime startDate, DateTime endDate, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(budgetId, startDate, endDate);
            return transactions.Select(t => MapToDto(t));
        }

        public async Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId)
        {
            var transactions = await _unitOfWork.Transactions.GetByUserIdAsync(userId);
            return transactions.Select(t => MapToDto(t));
        }

        public async Task<IEnumerable<TransactionDto>> FilterTransactionsAsync(TransactionFilterDto filterDto, int userId)
        {
            // Початковий список всіх доступних транзакцій
            IEnumerable<Transaction> transactions;

            if (filterDto.BudgetId.HasValue)
            {
                // Перевірка, чи має користувач доступ до бюджету
                if (!await _budgetService.HasUserAccessToBudgetAsync(filterDto.BudgetId.Value, userId))
                {
                    throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
                }
                transactions = await _unitOfWork.Transactions.GetByBudgetIdAsync(filterDto.BudgetId.Value);
            }
            else
            {
                // Отримати транзакції з усіх бюджетів, до яких користувач має доступ
                var families = await _unitOfWork.Users.GetUserFamiliesAsync(userId);
                var budgetIds = new List<int>();

                foreach (var family in families)
                {
                    var familyBudgets = await _unitOfWork.Budgets.GetByFamilyIdAsync(family.Id);
                    budgetIds.AddRange(familyBudgets.Select(b => b.Id));
                }

                transactions = new List<Transaction>();
                foreach (var budgetId in budgetIds)
                {
                    var budgetTransactions = await _unitOfWork.Transactions.GetByBudgetIdAsync(budgetId);
                    transactions = transactions.Concat(budgetTransactions);
                }
            }

            // Застосування фільтрів
            if (filterDto.CategoryId.HasValue)
            {
                transactions = transactions.Where(t => t.CategoryId == filterDto.CategoryId.Value);
            }

            if (filterDto.StartDate.HasValue)
            {
                transactions = transactions.Where(t => t.Date >= filterDto.StartDate.Value);
            }

            if (filterDto.EndDate.HasValue)
            {
                transactions = transactions.Where(t => t.Date <= filterDto.EndDate.Value);
            }

            if (filterDto.CreatedByUserId.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedByUserId == filterDto.CreatedByUserId.Value);
            }

            if (filterDto.MinAmount.HasValue)
            {
                transactions = transactions.Where(t => t.Amount >= filterDto.MinAmount.Value);
            }

            if (filterDto.MaxAmount.HasValue)
            {
                transactions = transactions.Where(t => t.Amount <= filterDto.MaxAmount.Value);
            }

             if (!string.IsNullOrEmpty(filterDto.SearchTerm))
            {
                transactions = transactions.Where(t => t.Description?.Contains(filterDto.SearchTerm, StringComparison.OrdinalIgnoreCase) == true);
            }

            return transactions.OrderByDescending(t => t.Date).Select(t => MapToDto(t));
        }

        public async Task<string> UploadReceiptImageAsync(int transactionId, Stream imageStream, string fileName, int userId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Транзакція не знайдена");
            }

            // Перевірка, чи має користувач право оновлювати транзакцію
            if (transaction.CreatedByUserId != userId)
            {
                var budget = await _unitOfWork.Budgets.GetByIdAsync(transaction.BudgetId);
                var userRole = await _familyService.GetUserRoleInFamilyAsync(budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"), userId);

                if (userRole != FamilyRole.Administrator)
                {
                    throw new UnauthorizedAccessException("У вас немає прав для оновлення цієї транзакції");
                }
            }

            // Тут має бути логіка завантаження файлу на сервер або хмарне сховище
            // Поверне URL доступу до зображення
            var imageUrl = $"/images/receipts/{transactionId}/{fileName}"; // Приклад URL

            transaction.ReceiptImageUrl = imageUrl;
            transaction.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Transactions.Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            return imageUrl;
        }

        #region Helper Methods

        private TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                ReceiptImageUrl = transaction.ReceiptImageUrl,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.Name ?? string.Empty,
                BudgetId = transaction.BudgetId,
                CreatedByUserId = transaction.CreatedByUserId,
                CreatedByUserName = transaction.CreatedByUser != null
                    ? $"{transaction.CreatedByUser.FirstName} {transaction.CreatedByUser.LastName}"
                    : string.Empty
            };
        }

        private async Task CheckBudgetLimitAndNotifyAsync(Transaction transaction, int userId)
        {
            // Отримання категорії транзакції
            var category = await _unitOfWork.Categories.GetByIdAsync(transaction.CategoryId);
            if (category == null || category.Type == CategoryType.Income) return; // Перевіряємо тільки витрати

            // Отримання активних лімітів для категорії
            var budgetLimits = await _unitOfWork.BudgetLimits.GetActiveAsync(transaction.BudgetId, DateTime.UtcNow);
            var categoryLimit = budgetLimits.FirstOrDefault(bl => bl.CategoryId == transaction.CategoryId);

            if (categoryLimit == null) return; // Немає ліміту для перевірки

            // Обчислення загальної суми витрат за період
            var transactions = await _unitOfWork.Transactions.GetByCategoryIdAsync(transaction.CategoryId);
            var totalSpent = transactions
                .Where(t => t.Date >= categoryLimit.StartDate && t.Date <= categoryLimit.EndDate)
                .Sum(t => t.Amount);

            // Перевірка, чи перевищено ліміт
            if (totalSpent > categoryLimit.Amount * 0.8m) // Попередження при досягненні 80% ліміту
            {
                await _notificationService.CreateLimitWarningNotificationAsync(
                    transaction.CategoryId,
                    transaction.BudgetId,
                    userId);
            }
        }

        #endregion
    }
}
