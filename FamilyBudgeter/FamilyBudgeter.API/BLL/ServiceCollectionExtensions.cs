using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.BLL.Services;

namespace FamilyBudgeter.API.BLL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLogicLayer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Реєстрація сервісів BLL
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFamilyService, FamilyService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IBudgetLimitService, BudgetLimitService>();
            services.AddScoped<IFinancialGoalService, FinancialGoalService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRegularPaymentService, RegularPaymentService>();
            services.AddScoped<IAnalysisService, AnalysisService>();

            return services;
        }
    }
}
