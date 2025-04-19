using FamilyBudgeter.API.Domain.Entities;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Отримати користувача за email
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Отримати всі сім'ї, до яких належить користувач
        /// </summary>
        Task<IEnumerable<Family>> GetUserFamiliesAsync(int userId);
    }
}
