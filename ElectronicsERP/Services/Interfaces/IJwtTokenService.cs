using ElectronicsERP.Models;

namespace ElectronicsERP.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, List<string> permissions);
    }
}
