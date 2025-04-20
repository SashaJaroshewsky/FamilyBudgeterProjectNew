using FamilyBudgeter.API.BLL.DTOs.FamilyDTOs;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface IFamilyService
    {
        /// <summary>
        /// Отримання всіх сімей користувача
        /// </summary>
        Task<IEnumerable<FamilyDto>> GetUserFamiliesAsync(int userId);

        /// <summary>
        /// Отримання сім'ї за ідентифікатором
        /// </summary>
        Task<FamilyDto> GetFamilyByIdAsync(int familyId, int userId);

        /// <summary>
        /// Створення нової сім'ї
        /// </summary>
        Task<FamilyDto> CreateFamilyAsync(CreateFamilyDto familyDto, int creatorUserId);

        /// <summary>
        /// Оновлення інформації про сім'ю
        /// </summary>
        Task<FamilyDto> UpdateFamilyAsync(int familyId, UpdateFamilyDto familyDto, int userId);

        /// <summary>
        /// Видалення сім'ї
        /// </summary>
        Task<bool> DeleteFamilyAsync(int familyId, int userId);

        /// <summary>
        /// Генерація нового коду приєднання
        /// </summary>
        Task<string> RegenerateJoinCodeAsync(int familyId, int userId);

        /// <summary>
        /// Приєднання до сім'ї за кодом
        /// </summary>
        Task<FamilyDto> JoinFamilyByCodeAsync(string joinCode, int userId);

        /// <summary>
        /// Отримання всіх членів сім'ї
        /// </summary>
        Task<IEnumerable<FamilyMemberDto>> GetFamilyMembersAsync(int familyId, int userId);

        /// <summary>
        /// Зміна ролі члена сім'ї
        /// </summary>
        Task<FamilyMemberDto> UpdateMemberRoleAsync(int familyId, FamilyMemberUpdateDto memberDto, int adminUserId);

        /// <summary>
        /// Видалення члена сім'ї
        /// </summary>
        Task<bool> RemoveFamilyMemberAsync(int familyId, int memberUserId, int adminUserId);

        /// <summary>
        /// Вихід із сім'ї
        /// </summary>
        Task<bool> LeaveFamilyAsync(int familyId, int userId);

        /// <summary>
        /// Перевірка, чи є користувач адміністратором сім'ї
        /// </summary>
        Task<bool> IsUserFamilyAdminAsync(int familyId, int userId);

        /// <summary>
        /// Перевірка, чи є користувач учасником сім'ї
        /// </summary>
        Task<bool> IsUserFamilyMemberAsync(int familyId, int userId);

        /// <summary>
        /// Отримання ролі користувача в сім'ї
        /// </summary>
        Task<FamilyRole?> GetUserRoleInFamilyAsync(int familyId, int userId);
    }
}
