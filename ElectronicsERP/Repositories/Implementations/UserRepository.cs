using ElectronicsERP.Data;
using ElectronicsERP.Models;
using ElectronicsERP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsERP.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExistsAsync(string username, CancellationToken ct)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username == username, ct);
        }

        public async Task<User?> GetUserWithRoleAsync(string username, CancellationToken ct)
        {
            return await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Username == username, ct);
        }

        public async Task AddUserAsync(User user, CancellationToken ct)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);
        }

    }
}