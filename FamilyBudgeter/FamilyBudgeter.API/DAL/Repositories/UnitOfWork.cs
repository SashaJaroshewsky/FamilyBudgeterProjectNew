using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IUserRepository? _userRepository;
        private IFamilyRepository? _familyRepository;
        private IFamilyMemberRepository? _familyMemberRepository;
        private IBudgetRepository? _budgetRepository;
        private ICategoryRepository? _categoryRepository;
        private ITransactionRepository? _transactionRepository;
        private IBudgetLimitRepository? _budgetLimitRepository;
        private IFinancialGoalRepository? _financialGoalRepository;
        private INotificationRepository? _notificationRepository;
        private IRegularPaymentRepository? _regularPaymentRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users =>
            _userRepository ??= new UserRepository(_context);

        public IFamilyRepository Families =>
            _familyRepository ??= new FamilyRepository(_context);

        public IFamilyMemberRepository FamilyMembers =>
            _familyMemberRepository ??= new FamilyMemberRepository(_context);

        public IBudgetRepository Budgets =>
            _budgetRepository ??= new BudgetRepository(_context);

        public ICategoryRepository Categories =>
            _categoryRepository ??= new CategoryRepository(_context);

        public ITransactionRepository Transactions =>
            _transactionRepository ??= new TransactionRepository(_context);

        public IBudgetLimitRepository BudgetLimits =>
            _budgetLimitRepository ??= new BudgetLimitRepository(_context);

        public IFinancialGoalRepository FinancialGoals =>
            _financialGoalRepository ??= new FinancialGoalRepository(_context);

        public INotificationRepository Notifications =>
            _notificationRepository ??= new NotificationRepository(_context);

        public IRegularPaymentRepository RegularPayments =>
            _regularPaymentRepository ??= new RegularPaymentRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
