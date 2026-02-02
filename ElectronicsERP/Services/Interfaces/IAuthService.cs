using ElectronicsERP.Models;

namespace ElectronicsERP.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request, CancellationToken ct);
        Task<string> LoginAsync(LoginRequest request, CancellationToken ct);
    }
}