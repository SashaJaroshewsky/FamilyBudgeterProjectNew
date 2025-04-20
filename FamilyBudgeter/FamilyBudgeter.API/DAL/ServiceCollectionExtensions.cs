using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessLayer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Реєстрація DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                   ));

            // Реєстрація репозиторіїв
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFamilyRepository, FamilyRepository>();
            services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IBudgetLimitRepository, BudgetLimitRepository>();
            services.AddScoped<IFinancialGoalRepository, FinancialGoalRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IRegularPaymentRepository, RegularPaymentRepository>();

            // Реєстрація UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
