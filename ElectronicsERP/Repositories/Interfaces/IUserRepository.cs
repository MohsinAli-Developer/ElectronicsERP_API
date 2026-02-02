using ElectronicsERP.Models;

namespace ElectronicsERP.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserExistsAsync(string username, CancellationToken ct);
        Task<User?> GetUserWithRoleAsync(string username, CancellationToken ct);
        Task AddUserAsync(User user, CancellationToken ct);
    }
}
