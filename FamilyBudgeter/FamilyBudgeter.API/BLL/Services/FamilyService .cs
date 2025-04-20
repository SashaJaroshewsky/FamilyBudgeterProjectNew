using FamilyBudgeter.API.BLL.DTOs.FamilyDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.Services
{
    public class FamilyService : IFamilyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FamilyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FamilyDto>> GetUserFamiliesAsync(int userId)
        {
            // Використовуємо правильний метод для отримання сімей користувача
            var families = await _unitOfWork.Users.GetUserFamiliesAsync(userId);
            var result = new List<FamilyDto>();

            foreach (var family in families)
            {
                var familyWithMembers = await _unitOfWork.Families.GetWithMembersAsync(family.Id);
                if (familyWithMembers != null)
                {
                    result.Add(MapFamilyToDto(familyWithMembers));
                }
            }

            return result;
        }

        public async Task<FamilyDto> GetFamilyByIdAsync(int familyId, int userId)
        {
            // Перевірка, чи належить користувач до сім'ї
            if (!await IsUserFamilyMemberAsync(familyId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цієї сім'ї");
            }

            var family = await _unitOfWork.Families.GetWithMembersAsync(familyId);
            if (family == null)
            {
                throw new KeyNotFoundException("Сім'я не знайдена");
            }

            return MapFamilyToDto(family);
        }

        public async Task<FamilyDto> CreateFamilyAsync(CreateFamilyDto familyDto, int creatorUserId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(creatorUserId);
            if (user == null)
            {
                throw new KeyNotFoundException("Користувач не знайдений");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Створення сім'ї
                var family = new Family
                {
                    Name = familyDto.Name,
                    JoinCode = GenerateJoinCode()
                };

                await _unitOfWork.Families.AddAsync(family);
                await _unitOfWork.SaveChangesAsync();

                // Додавання користувача як адміністратора сім'ї
                var familyMember = new FamilyMember
                {
                    UserId = creatorUserId,
                    FamilyId = family.Id,
                    Role = FamilyRole.Administrator
                };

                await _unitOfWork.FamilyMembers.AddAsync(familyMember);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                var createdFamily = await _unitOfWork.Families.GetWithMembersAsync(family.Id);
                return MapFamilyToDto(createdFamily!);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<FamilyDto> UpdateFamilyAsync(int familyId, UpdateFamilyDto familyDto, int userId)
        {
            // Перевірка, чи є користувач адміністратором сім'ї
            if (!await IsUserFamilyAdminAsync(familyId, userId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може оновлювати інформацію");
            }

            var family = await _unitOfWork.Families.GetByIdAsync(familyId);
            if (family == null)
            {
                throw new KeyNotFoundException("Сім'я не знайдена");
            }

            family.Name = familyDto.Name;
            family.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Families.Update(family);
            await _unitOfWork.SaveChangesAsync();

            var updatedFamily = await _unitOfWork.Families.GetWithMembersAsync(familyId);
            return MapFamilyToDto(updatedFamily!);
        }

        public async Task<bool> DeleteFamilyAsync(int familyId, int userId)
        {
            // Перевірка, чи є користувач адміністратором сім'ї
            if (!await IsUserFamilyAdminAsync(familyId, userId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може видаляти сім'ю");
            }

            var family = await _unitOfWork.Families.GetByIdAsync(familyId);
            if (family == null)
            {
                throw new KeyNotFoundException("Сім'я не знайдена");
            }

            // Видалення всіх членів сім'ї
            var members = await _unitOfWork.FamilyMembers.GetByFamilyIdAsync(familyId);
            foreach (var member in members)
            {
                _unitOfWork.FamilyMembers.Delete(member);
            }

            // Видалення сім'ї
            await _unitOfWork.Families.DeleteAsync(familyId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<string> RegenerateJoinCodeAsync(int familyId, int userId)
        {
            // Перевірка, чи є користувач адміністратором сім'ї
            if (!await IsUserFamilyAdminAsync(familyId, userId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може регенерувати код приєднання");
            }

            var family = await _unitOfWork.Families.GetByIdAsync(familyId);
            if (family == null)
            {
                throw new KeyNotFoundException("Сім'я не знайдена");
            }

            family.JoinCode = GenerateJoinCode();
            family.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Families.Update(family);
            await _unitOfWork.SaveChangesAsync();

            return family.JoinCode;
        }

        public async Task<FamilyDto> JoinFamilyByCodeAsync(string joinCode, int userId)
        {
            var family = await _unitOfWork.Families.GetByJoinCodeAsync(joinCode);
            if (family == null)
            {
                throw new KeyNotFoundException("Сім'я з таким кодом приєднання не знайдена");
            }

            // Перевірка, чи вже є користувач членом сім'ї
            var existingMember = await _unitOfWork.FamilyMembers.GetByUserAndFamilyIdAsync(userId, family.Id);
            if (existingMember != null)
            {
                throw new InvalidOperationException("Ви вже є членом цієї сім'ї");
            }

            // Додавання користувача як повного учасника сім'ї
            var familyMember = new FamilyMember
            {
                UserId = userId,
                FamilyId = family.Id,
                Role = FamilyRole.FullMember
            };

            await _unitOfWork.FamilyMembers.AddAsync(familyMember);
            await _unitOfWork.SaveChangesAsync();

            var joinedFamily = await _unitOfWork.Families.GetWithMembersAsync(family.Id);
            return MapFamilyToDto(joinedFamily!);
        }

        public async Task<IEnumerable<FamilyMemberDto>> GetFamilyMembersAsync(int familyId, int userId)
        {
            // Перевірка, чи належить користувач до сім'ї
            if (!await IsUserFamilyMemberAsync(familyId, userId))
            {
                throw new UnauthorizedAccessException("У вас немає доступу до цієї сім'ї");
            }

            var members = await _unitOfWork.FamilyMembers.GetByFamilyIdAsync(familyId);
            var result = members.Select(m => new FamilyMemberDto
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.User != null ? $"{m.User.FirstName} {m.User.LastName}" : "Невідомий користувач",
                UserEmail = m.User?.Email ?? "Невідома електронна пошта",
                Role = m.Role
            });

            return result;
        }

        public async Task<FamilyMemberDto> UpdateMemberRoleAsync(int familyId, FamilyMemberUpdateDto memberDto, int adminUserId)
        {
            // Перевірка, чи є користувач адміністратором сім'ї
            if (!await IsUserFamilyAdminAsync(familyId, adminUserId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може змінювати ролі членів");
            }

            // Перевірка, чи є змінюваний користувач єдиним адміністратором
            if (memberDto.Role != FamilyRole.Administrator)
            {
                var admins = await _unitOfWork.FamilyMembers.GetByRoleAsync(familyId, FamilyRole.Administrator);
                if (admins.Count() == 1 && admins.First().UserId == memberDto.UserId)
                {
                    throw new InvalidOperationException("Не можна змінити роль єдиного адміністратора сім'ї");
                }
            }

            var member = await _unitOfWork.FamilyMembers.GetByUserAndFamilyIdAsync(memberDto.UserId, familyId);
            if (member == null)
            {
                throw new KeyNotFoundException("Член сім'ї не знайдений");
            }

            member.Role = memberDto.Role;
            member.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.FamilyMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            // Отримання оновленого члена сім'ї
            var members = await _unitOfWork.FamilyMembers.GetByFamilyIdAsync(familyId);
            var updatedMember = members.FirstOrDefault(m => m.UserId == memberDto.UserId);

            if (updatedMember == null || updatedMember.User == null)
            if (updatedMember == null || updatedMember.User == null)
            {
                throw new KeyNotFoundException("Член сім'ї не знайдений");
            }

            return new FamilyMemberDto
            {
                Id = updatedMember.Id,
                UserId = updatedMember.UserId,
                UserName = $"{updatedMember.User.FirstName} {updatedMember.User.LastName}",
                UserEmail = updatedMember.User.Email,
                Role = updatedMember.Role
            };
        }

        public async Task<bool> RemoveFamilyMemberAsync(int familyId, int memberUserId, int adminUserId)
        {
            // Перевірка, чи є користувач адміністратором сім'ї
            if (!await IsUserFamilyAdminAsync(familyId, adminUserId))
            {
                throw new UnauthorizedAccessException("Тільки адміністратор сім'ї може видаляти членів");
            }

            // Не можна видалити самого себе
            if (memberUserId == adminUserId)
            {
                throw new InvalidOperationException("Ви не можете видалити себе. Для виходу із сім'ї використовуйте відповідну функцію");
            }

            var member = await _unitOfWork.FamilyMembers.GetByUserAndFamilyIdAsync(memberUserId, familyId);
            if (member == null)
            {
                throw new KeyNotFoundException("Член сім'ї не знайдений");
            }

            _unitOfWork.FamilyMembers.Delete(member);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LeaveFamilyAsync(int familyId, int userId)
        {
            // Перевірка, чи є користувач членом сім'ї
            var member = await _unitOfWork.FamilyMembers.GetByUserAndFamilyIdAsync(userId, familyId);
            if (member == null)
            {
                throw new KeyNotFoundException("Ви не є членом цієї сім'ї");
            }

            // Перевірка, чи є користувач єдиним адміністратором
            if (member.Role == FamilyRole.Administrator)
            {
                var admins = await _unitOfWork.FamilyMembers.GetByRoleAsync(familyId, FamilyRole.Administrator);
                if (admins.Count() == 1)
                {
                    throw new InvalidOperationException("Ви не можете вийти із сім'ї, оскільки є єдиним адміністратором. Призначте іншого адміністратора або видаліть сім'ю");
                }
            }

            _unitOfWork.FamilyMembers.Delete(member);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsUserFamilyAdminAsync(int familyId, int userId)
        {
            return await _unitOfWork.FamilyMembers.IsAdminAsync(userId, familyId);
        }

        public async Task<bool> IsUserFamilyMemberAsync(int familyId, int userId)
        {
            var member = await _unitOfWork.FamilyMembers.GetByUserAndFamilyIdAsync(userId, familyId);
            return member != null;
        }

        public async Task<FamilyRole?> GetUserRoleInFamilyAsync(int familyId, int userId)
        {
            var member = await _unitOfWork.FamilyMembers.GetByUserAndFamilyIdAsync(userId, familyId);
            return member?.Role;
        }

        #region Helper Methods

        private FamilyDto MapFamilyToDto(Family family)
        {
            return new FamilyDto
            {
                Id = family.Id,
                Name = family.Name,
                JoinCode = family.JoinCode,
                Members = family.Members.Select(m => new FamilyMemberDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    UserName = m.User != null ? $"{m.User.FirstName} {m.User.LastName}" : "Невідомий користувач",
                    UserEmail = m.User?.Email ?? "Невідома електронна пошта",
                    Role = m.Role
                }).ToList()
            };
        }

        private string GenerateJoinCode()
        {
            // Генерація 8-символьного буквено-цифрового коду
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return code;
        }

        #endregion
    }
}
