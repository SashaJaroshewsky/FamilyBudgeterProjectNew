namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IFamilyRepository Families { get; }
        IFamilyMemberRepository FamilyMembers { get; }
        IBudgetRepository Budgets { get; }
        ICategoryRepository Categories { get; }
        ITransactionRepository Transactions { get; }
        IBudgetLimitRepository BudgetLimits { get; }
        IFinancialGoalRepository FinancialGoals { get; }
        INotificationRepository Notifications { get; }
        IRegularPaymentRepository RegularPayments { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
