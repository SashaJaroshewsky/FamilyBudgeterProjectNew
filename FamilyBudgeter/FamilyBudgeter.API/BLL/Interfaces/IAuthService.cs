using FamilyBudgeter.API.BLL.DTOs.AuthDTOs;

namespace FamilyBudgeter.API.BLL.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Реєстрація нового користувача
        /// </summary>
        Task<AuthResponseDto> RegisterAsync(UserRegistrationDto registrationDto);

        /// <summary>
        /// Аутентифікація користувача
        /// </summary>
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);

        /// <summary>
        /// Оновлення токену
        /// </summary>
        Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);

        /// <summary>
        /// Отримання інформації про поточного користувача
        /// </summary>
        Task<UserDto> GetCurrentUserAsync(int userId);

        /// <summary>
        /// Оновлення інформації користувача
        /// </summary>
        Task<UserDto> UpdateUserAsync(int userId, UserDto userDto);

        /// <summary>
        /// Зміна пароля користувача
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
