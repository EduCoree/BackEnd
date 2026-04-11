using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly EduCoreDbContext _context;

        public AuthenticationRepository(EduCoreDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.Set<RefreshToken>()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(string userId)
        {
            return await _context.Set<RefreshToken>()
                .Where(r => r.UserId == userId && !r.IsRevoked)
                .ToListAsync();
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.Set<RefreshToken>().AddAsync(refreshToken);
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.Set<RefreshToken>().Update(refreshToken);
        }
    }
}
