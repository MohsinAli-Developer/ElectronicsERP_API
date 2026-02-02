using ElectronicsERP.Data;
using ElectronicsERP.Models;
using ElectronicsERP.Repositories.Interfaces;
using ElectronicsERP.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsERP.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtService;
        private readonly ApplicationDbContext _context;

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenService jwtService,
            ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _context = context;
        }

        public async Task RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            if (await _userRepository.UserExistsAsync(request.Name, ct))
                throw new Exception("Username already exists");

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == request.Role, ct);

            if (role == null)
                throw new Exception("Invalid role");

            var user = new User
            {
                Username = request.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = role.Id
            };

            await _userRepository.AddUserAsync(user, ct);
        }

        public async Task<string> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            var user = await _userRepository.GetUserWithRoleAsync(request.Username, ct);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var permissions = user.Role.RolePermissions
                .Where(rp => rp.IsAllowed)
                .Select(rp => rp.Permission.Name)
                .ToList();

            return _jwtService.GenerateToken(user, permissions);
        }
    }
}
