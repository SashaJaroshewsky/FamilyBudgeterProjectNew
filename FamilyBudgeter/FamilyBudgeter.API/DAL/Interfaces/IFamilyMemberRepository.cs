using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface IFamilyMemberRepository : IGenericRepository<FamilyMember>
    {
        /// <summary>
        /// Отримати членів сім'ї за ідентифікатором сім'ї
        /// </summary>
        Task<IEnumerable<FamilyMember>> GetByFamilyIdAsync(int familyId);

        /// <summary>
        /// Отримати члена сім'ї за ідентифікатором користувача та сім'ї
        /// </summary>
        Task<FamilyMember?> GetByUserAndFamilyIdAsync(int userId, int familyId);

        /// <summary>
        /// Отримати всіх членів сім'ї з певною роллю
        /// </summary>
        Task<IEnumerable<FamilyMember>> GetByRoleAsync(int familyId, FamilyRole role);

        /// <summary>
        /// Перевірити чи є користувач адміністратором сім'ї
        /// </summary>
        Task<bool> IsAdminAsync(int userId, int familyId);
    }
}
