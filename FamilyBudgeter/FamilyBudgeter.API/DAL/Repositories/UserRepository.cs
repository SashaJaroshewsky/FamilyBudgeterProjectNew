using FamilyBudgeter.API.DAL.Context;
using FamilyBudgeter.API.DAL.Interfaces;
using FamilyBudgeter.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudgeter.API.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Family>> GetUserFamiliesAsync(int userId)
        {
            return await _context.FamilyMembers
                .Where(fm => fm.UserId == userId)
                .Select(fm => fm.Family)
                .ToListAsync();
        }
    }
}
