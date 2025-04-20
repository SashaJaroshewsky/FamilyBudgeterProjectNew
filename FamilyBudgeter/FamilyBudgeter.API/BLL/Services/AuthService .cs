using FamilyBudgeter.API.BLL.DTOs.AuthDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FamilyBudgeter.API.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                // Перевірка чи існує користувач з такою електронною поштою
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(registrationDto.Email);
                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Користувач з такою електронною поштою вже існує"
                    };
                }

                // Створення нового користувача
                var user = new User
                {
                    Email = registrationDto.Email,
                    PasswordHash = HashPassword(registrationDto.Password),
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Генерація токену
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Повернення результату
                return new AuthResponseDto
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"] ?? "60")),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Помилка при реєстрації: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                // Пошук користувача за електронною поштою
                var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Користувач з такою електронною поштою не знайдений"
                    };
                }

                // Перевірка пароля
                if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Невірний пароль"
                    };
                }

                // Генерація токену
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Повернення результату
                return new AuthResponseDto
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"] ?? "60")),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Помилка при авторизації: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(token);
                if (principal == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Недійсний або прострочений токен"
                    };
                }

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Недійсний токен"
                    };
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Користувач не знайдений"
                    };
                }

                // У реальному додатку тут має бути перевірка refreshToken в базі даних
                // Для спрощення ми просто генеруємо новий токен

                var newToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                return new AuthResponseDto
                {
                    Success = true,
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"] ?? "60")),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Помилка при оновленні токену: {ex.Message}"
                };
            }
        }

        public async Task<UserDto> GetCurrentUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Користувач не знайдений");
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UserDto userDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Користувач не знайдений");
            }

            // Перевірка, чи змінилася електронна пошта
            if (user.Email != userDto.Email)
            {
                // Перевірка, чи існує користувач з такою електронною поштою
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(userDto.Email);
                if (existingUser != null && existingUser.Id != userId)
                {
                    throw new InvalidOperationException("Користувач з такою електронною поштою вже існує");
                }

                user.Email = userDto.Email;
            }

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return userDto;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Користувач не знайдений");
            }

            // Перевірка поточного пароля
            if (!VerifyPassword(currentPassword, user.PasswordHash))
            {
                throw new InvalidOperationException("Поточний пароль невірний");
            }

            // Оновлення пароля
            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #region Helper Methods

        private string HashPassword(string password)
        {
            // Використовуємо BCrypt або подібну бібліотеку для хешування паролів
            // Для спрощення використаємо простий хеш-алгоритм
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Для спрощення використаємо простий хеш-алгоритм
            var hashedPassword = HashPassword(password);
            return hashedPassword == passwordHash;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "defaultsecretkey12345678901234567890");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"] ?? "60")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "defaultsecretkey12345678901234567890");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = false // Дозволяємо прострочені токени
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }

        #endregion
    }
}
