using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class FamilyMemberRepository : GenericRepository<FamilyMember>, IFamilyMemberRepository
    {
        public FamilyMemberRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FamilyMember>> GetByFamilyIdAsync(int familyId)
        {
            return await _dbSet
                .Include(fm => fm.User)
                .Where(fm => fm.FamilyId == familyId)
                .ToListAsync();
        }

        public async Task<FamilyMember?> GetByUserAndFamilyIdAsync(int userId, int familyId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(fm => fm.UserId == userId && fm.FamilyId == familyId);
        }

        public async Task<IEnumerable<FamilyMember>> GetByRoleAsync(int familyId, FamilyRole role)
        {
            return await _dbSet
                .Include(fm => fm.User)
                .Where(fm => fm.FamilyId == familyId && fm.Role == role)
                .ToListAsync();
        }

        public async Task<bool> IsAdminAsync(int userId, int familyId)
        {
            var member = await _dbSet
                .FirstOrDefaultAsync(fm => fm.UserId == userId && fm.FamilyId == familyId);

            return member != null && member.Role == FamilyRole.Administrator;
        }
    }
}
