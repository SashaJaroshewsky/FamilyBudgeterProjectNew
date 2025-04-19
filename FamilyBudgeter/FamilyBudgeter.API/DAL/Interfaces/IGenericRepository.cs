using FamilyBudgeter.API.Domain.Entities;
using System.Linq.Expressions;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Отримати всі об'єкти
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Отримати об'єкт за ідентифікатором
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Отримати об'єкти за певною умовою
        /// </summary>
        Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Додати новий об'єкт
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Додати колекцію об'єктів
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Оновити існуючий об'єкт
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Видалити об'єкт за ідентифікатором
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Видалити існуючий об'єкт
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Перевірити існування об'єкта за певною умовою
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
