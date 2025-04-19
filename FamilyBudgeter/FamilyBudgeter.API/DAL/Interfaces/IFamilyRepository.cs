using FamilyBudgeter.API.Domain.Entities;

namespace FamilyBudgeter.API.DAL.Interfaces
{
    public interface IFamilyRepository : IGenericRepository<Family>
    {
        /// <summary>
        /// Отримати сім'ю з усіма її членами
        /// </summary>
        Task<Family?> GetWithMembersAsync(int familyId);

        /// <summary>
        /// Отримати сім'ю за кодом приєднання
        /// </summary>
        Task<Family?> GetByJoinCodeAsync(string joinCode);

        /// <summary>
        /// Отримати сім'ю з усіма бюджетами
        /// </summary>
        Task<Family?> GetWithBudgetsAsync(int familyId);
    }
}
