using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class FamilyRepository : GenericRepository<Family>, IFamilyRepository
    {
        public FamilyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Family?> GetWithMembersAsync(int familyId)
        {
            return await _dbSet
                .Include(f => f.Members)
                    .ThenInclude(fm => fm.User)
                .FirstOrDefaultAsync(f => f.Id == familyId);
        }

        public async Task<Family?> GetByJoinCodeAsync(string joinCode)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.JoinCode == joinCode);
        }

        public async Task<Family?> GetWithBudgetsAsync(int familyId)
        {
            return await _dbSet
                .Include(f => f.Budgets)
                .FirstOrDefaultAsync(f => f.Id == familyId);
        }
    }
}
