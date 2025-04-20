using FamilyBudgeter.API.BLL.DTOs.RegularPaymentDTOs;
using FamilyBudgeter.API.BLL.DTOs.TransactionDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Services
{
    public class RegularPaymentService : IRegularPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFamilyService _familyService;
        private readonly IBudgetService _budgetService;
        private readonly INotificationService _notificationService;
        private readonly ITransactionService _transactionService;

        public RegularPaymentService(
            IUnitOfWork unitOfWork,
            IFamilyService familyService,
            IBudgetService budgetService,
            INotificationService notificationService,
            ITransactionService transactionService)
        {
            _unitOfWork = unitOfWork;
            _familyService = familyService;
            _budgetService = budgetService;
            _notificationService = notificationService;
            _transactionService = transactionService;
        }

        public async Task<IEnumerable<RegularPaymentDto>> GetBudgetRegularPaymentsAsync(int budgetId, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var payments = await _unitOfWork.RegularPayments.GetByBudgetIdAsync(budgetId);
            return payments.Select(p => MapToDto(p));
        }

        public async Task<RegularPaymentDto> GetRegularPaymentByIdAsync(int paymentId, int userId)
        {
            var payment = await _unitOfWork.RegularPayments.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new KeyNotFoundException("Регулярний платіж не знайдено");
            }

            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(payment.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього платежу");
            }

            return MapToDto(payment);
        }

        public async Task<RegularPaymentDto> CreateRegularPaymentAsync(CreateRegularPaymentDto paymentDto, int userId)
        {
            // Перевірка, чи належить користувач до сім'ї та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(paymentDto.BudgetId);
            if (budget == null)
            {
                throw new KeyNotFoundException("Бюджет не знайдено");
            }

            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget.FamilyId, userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для створення регулярних платежів в цьому бюджеті");
            }

            // Перевірка, чи належить категорія до цього бюджету
            var category = await _unitOfWork.Categories.GetByIdAsync(paymentDto.CategoryId);
            if (category == null || category.BudgetId != paymentDto.BudgetId)
            {
                throw new InvalidOperationException("Категорія не належить до цього бюджету");
            }

            // Перевірка, що дата початку не в минулому
            if (paymentDto.StartDate.Date < DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException("Дата початку не може бути в минулому");
            }

            // Перевірка, що дата закінчення після дати початку (якщо вказана)
            if (paymentDto.EndDate.HasValue && paymentDto.EndDate.Value <= paymentDto.StartDate)
            {
                throw new InvalidOperationException("Дата закінчення має бути після дати початку");
            }

            var payment = new RegularPayment
            {
                Name = paymentDto.Name,
                Amount = paymentDto.Amount,
                Description = paymentDto.Description,
                StartDate = paymentDto.StartDate,
                EndDate = paymentDto.EndDate,
                Frequency = paymentDto.Frequency,
                DayOfMonth = paymentDto.DayOfMonth,
                CategoryId = paymentDto.CategoryId,
                BudgetId = paymentDto.BudgetId
            };

            await _unitOfWork.RegularPayments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(payment);
        }

        public async Task<RegularPaymentDto> UpdateRegularPaymentAsync(int paymentId, UpdateRegularPaymentDto paymentDto, int userId)
        {
            var payment = await _unitOfWork.RegularPayments.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new KeyNotFoundException("Регулярний платіж не знайдено");
            }

            // Перевірка, чи належить користувач до сім'ї та чи є він адміністратором або повним учасником
            var budget = await _unitOfWork.Budgets.GetByIdAsync(payment.BudgetId);
            var userRole = await _familyService.GetUserRoleInFamilyAsync(budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"), userId);
            if (userRole == null || userRole == FamilyRole.LimitedMember)
            {
                throw new UnauthorizedAccessException("У вас немає прав для оновлення регулярних платежів в цьому бюджеті");
            }

            // Перевірка, чи належить категорія до цього бюджету
            var category = await _unitOfWork.Categories.GetByIdAsync(paymentDto.CategoryId);
            if (category == null || category.BudgetId != payment.BudgetId)
            {
                throw new InvalidOperationException("Категорія не належить до цього бюджету");
            }

            // Перевірка, що дата закінчення після дати початку (якщо вказана)
            if (paymentDto.EndDate.HasValue && paymentDto.EndDate.Value <= paymentDto.StartDate)
            {
                throw new InvalidOperationException("Дата закінчення має бути після дати початку");
            }

            payment.Name = paymentDto.Name;
            payment.Amount = paymentDto.Amount;
            payment.Description = paymentDto.Description;
            payment.StartDate = paymentDto.StartDate;
            payment.EndDate = paymentDto.EndDate;
            payment.Frequency = paymentDto.Frequency;
            payment.DayOfMonth = paymentDto.DayOfMonth;
            payment.CategoryId = paymentDto.CategoryId;
            payment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.RegularPayments.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(payment);
        }

        public async Task<bool> DeleteRegularPaymentAsync(int paymentId, int userId)
        {
            var payment = await _unitOfWork.RegularPayments.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new KeyNotFoundException("Регулярний платіж не знайдено");
            }

            // Перевірка, чи є користувач адміністратором сім'ї
            var budget = await _unitOfWork.Budgets.GetByIdAsync(payment.BudgetId);
            if (!await _familyService.IsUserFamilyAdminAsync(budget?.FamilyId ?? throw new KeyNotFoundException("Бюджет не знайдено"), userId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може видаляти регулярні платежі");
            }

            await _unitOfWork.RegularPayments.DeleteAsync(paymentId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RegularPaymentDto>> GetRegularPaymentsByCategoryAsync(int categoryId, int userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("Категорія не знайдена");
            }

            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(category.BudgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до платежів цієї категорії");
            }

            var payments = await _unitOfWork.RegularPayments.GetByBudgetIdAsync(category.BudgetId);
            var categoryPayments = payments.Where(p => p.CategoryId == categoryId);
            return categoryPayments.Select(p => MapToDto(p));
        }

        public async Task<IEnumerable<RegularPaymentDto>> GetRegularPaymentsByFrequencyAsync(int budgetId, PaymentFrequency frequency, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var payments = await _unitOfWork.RegularPayments.GetByFrequencyAsync(budgetId, frequency);
            return payments.Select(p => MapToDto(p));
        }

        public async Task<IEnumerable<RegularPaymentDto>> GetUpcomingRegularPaymentsAsync(int budgetId, DateTime startDate, DateTime endDate, int userId)
        {
            // Перевірка, чи має користувач доступ до бюджету
            if (!await _budgetService.HasUserAccessToBudgetAsync(budgetId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цього бюджету");
            }

            var activePayments = await _unitOfWork.RegularPayments.GetActiveAsync(budgetId, DateTime.UtcNow);
            var upcomingPayments = new List<RegularPaymentDto>();

            foreach (var payment in activePayments)
            {
                var paymentDates = CalculatePaymentDates(payment, startDate, endDate);
                if (paymentDates.Any())
                {
                    upcomingPayments.Add(MapToDto(payment));
                }
            }

            return upcomingPayments;
        }

        public async Task<int> ProcessDueRegularPaymentsAsync()
        {
            var processedCount = 0;
            var today = DateTime.UtcNow.Date;

            // Отримання всіх бюджетів
            var allBudgets = await _unitOfWork.Budgets.GetAllAsync();

            foreach (var budget in allBudgets)
            {
                // Отримання активних регулярних платежів для бюджету
                var activePayments = await _unitOfWork.RegularPayments.GetActiveAsync(budget.Id, today);

                foreach (var payment in activePayments)
                {
                    if (ShouldProcessPaymentToday(payment, today))
                    {
                        try
                        {
                            // Створення транзакції для регулярного платежу
                            var transactionDto = new CreateTransactionDto
                            {
                                Amount = payment.Amount,
                                Description = $"Регулярний платіж: {payment.Name}",
                                Date = today,
                                CategoryId = payment.CategoryId,
                                BudgetId = payment.BudgetId
                            };

                            // Знаходимо першого адміністратора сім'ї для створення транзакції
                            var adminMembers = await _unitOfWork.FamilyMembers.GetByRoleAsync(budget.FamilyId, FamilyRole.Administrator);
                            var adminUserId = adminMembers.FirstOrDefault()?.UserId ?? 0;

                            if (adminUserId == 0)
                            {
                                continue; // Немає адміністратора, пропускаємо
                            }

                            await _transactionService.CreateTransactionAsync(transactionDto, adminUserId);

                            // Відправка повідомлення про регулярний платіж
                            await _notificationService.CreateRegularPaymentNotificationAsync(payment.Id, adminUserId);

                            processedCount++;
                        }
                        catch (Exception ex)
                        {
                            // Логування помилки обробки платежу
                            // В реальному додатку тут має бути логування
                            Console.WriteLine($"Error processing regular payment {payment.Id}: {ex.Message}");
                        }
                    }
                }
            }

            return processedCount;
        }

        #region Helper Methods

        private RegularPaymentDto MapToDto(RegularPayment payment)
        {
            return new RegularPaymentDto
            {
                Id = payment.Id,
                Name = payment.Name,
                Amount = payment.Amount,
                Description = payment.Description,
                StartDate = payment.StartDate,
                EndDate = payment.EndDate,
                Frequency = payment.Frequency,
                DayOfMonth = payment.DayOfMonth,
                CategoryId = payment.CategoryId,
                CategoryName = payment.Category?.Name ?? string.Empty,
                BudgetId = payment.BudgetId
            };
        }

        private bool ShouldProcessPaymentToday(RegularPayment payment, DateTime today)
        {
            if (payment.StartDate > today || (payment.EndDate.HasValue && payment.EndDate.Value < today))
            {
                return false;
            }

            switch (payment.Frequency)
            {
                case PaymentFrequency.Daily:
                    return true;

                case PaymentFrequency.Weekly:
                    return payment.StartDate.DayOfWeek == today.DayOfWeek;

                case PaymentFrequency.BiWeekly:
                    var weeksDifference = (today - payment.StartDate.Date).TotalDays / 7;
                    return Math.Abs(weeksDifference % 2) < 0.1 && payment.StartDate.DayOfWeek == today.DayOfWeek;

                case PaymentFrequency.Monthly:
                    return today.Day == payment.DayOfMonth;

                case PaymentFrequency.Quarterly:
                    var months = new[] { 1, 4, 7, 10 }; // Місяці кварталів
                    return months.Contains(today.Month) && today.Day == payment.DayOfMonth;

                case PaymentFrequency.Yearly:
                    return today.Month == payment.StartDate.Month && today.Day == payment.DayOfMonth;

                default:
                    return false;
            }
        }

        private List<DateTime> CalculatePaymentDates(RegularPayment payment, DateTime startDate, DateTime endDate)
        {
            var dates = new List<DateTime>();
            var currentDate = payment.StartDate.Date;

            while (currentDate <= endDate && (!payment.EndDate.HasValue || currentDate <= payment.EndDate.Value))
            {
                if (currentDate >= startDate)
                {
                    dates.Add(currentDate);
                }

                switch (payment.Frequency)
                {
                    case PaymentFrequency.Daily:
                        currentDate = currentDate.AddDays(1);
                        break;

                    case PaymentFrequency.Weekly:
                        currentDate = currentDate.AddDays(7);
                        break;

                    case PaymentFrequency.BiWeekly:
                        currentDate = currentDate.AddDays(14);
                        break;

                    case PaymentFrequency.Monthly:
                        currentDate = currentDate.AddMonths(1);
                        // Корекція для місяців з меншою кількістю днів
                        if (currentDate.Day < payment.DayOfMonth && payment.DayOfMonth <= DateTime.DaysInMonth(currentDate.Year, currentDate.Month))
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, payment.DayOfMonth);
                        }
                        break;

                    case PaymentFrequency.Quarterly:
                        currentDate = currentDate.AddMonths(3);
                        if (currentDate.Day < payment.DayOfMonth && payment.DayOfMonth <= DateTime.DaysInMonth(currentDate.Year, currentDate.Month))
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, payment.DayOfMonth);
                        }
                        break;

                    case PaymentFrequency.Yearly:
                        currentDate = currentDate.AddYears(1);
                        if (currentDate.Day < payment.DayOfMonth && payment.DayOfMonth <= DateTime.DaysInMonth(currentDate.Year, currentDate.Month))
                        {
                            currentDate = new DateTime(currentDate.Year, currentDate.Month, payment.DayOfMonth);
                        }
                        break;

                    default:
                        return dates;
                }
            }

            return dates;
        }

        #endregion
    }
}
