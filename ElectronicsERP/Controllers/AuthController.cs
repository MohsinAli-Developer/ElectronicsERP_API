//using ElectronicsERP.Data;
//using ElectronicsERP.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using BCrypt.Net;

//namespace ElectronicsERP.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IConfiguration _configuration;

//        public AuthController(ApplicationDbContext context, IConfiguration configuration)
//        {
//            _context = context;
//            _configuration = configuration;
//        }

//        [HttpPost("register")]
//        public IActionResult Register([FromBody] RegisterRequest request)
//        {
//            if (request == null || string.IsNullOrWhiteSpace(request.Name) ||
//                string.IsNullOrWhiteSpace(request.Password) || request.Role <= 0)
//            {
//                return BadRequest(new { message = "Username, Role, and Password are required" });
//            }

//            if (_context.Users.Any(u => u.Username == request.Name))
//            {
//                return Conflict(new { message = "Username already exists" });
//            }

//            var role = _context.Roles.FirstOrDefault(r => r.Id == request.Role);
//            if (role == null)
//            {
//                return BadRequest(new { message = "Invalid role" });
//            }

//            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

//            var user = new User
//            {
//                Username = request.Name,
//                PasswordHash = hashedPassword,
//                RoleId = role.Id
//            };

//            _context.Users.Add(user);
//            _context.SaveChanges();

//            return Ok(new { message = "Registration successful" });
//        }

//        [HttpPost("login")]
//        public IActionResult Login([FromBody] LoginRequest request)
//        {
//            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
//            {
//                return BadRequest(new { message = "Username and password are required" });
//            }

//            var user = _context.Users
//                .Include(u => u.Role)
//                .ThenInclude(r => r.RolePermissions)
//                .ThenInclude(rp => rp.Permission)
//                .FirstOrDefault(u => u.Username == request.Username);

//            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
//            {
//                return Unauthorized(new { message = "Invalid username or password" });
//            }

//            // ✅ Get all allowed permissions for this role
//            var permissions = user.Role.RolePermissions
//                .Where(rp => rp.IsAllowed)
//                .Select(rp => rp.Permission.Name)
//                .ToList();

//            var token = GenerateJwtToken(user.Username, user.Role.Name, permissions);

//            return Ok(new
//            {
//                message = "Login successful",
//                token
//            });
//        }

//        private string GenerateJwtToken(string username, string role, List<string> permissions)
//        {
//            var jwtSettings = _configuration.GetSection("JwtSettings");
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, username),
//                new Claim(ClaimTypes.Role, role)
//            };

//            foreach (var permission in permissions)
//            {
//                claims.Add(new Claim("permission", permission));
//            }

//            var token = new JwtSecurityToken(
//                issuer: jwtSettings["Issuer"],
//                audience: jwtSettings["Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(2),
//                signingCredentials: creds
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}




using ElectronicsERP.Models;
using ElectronicsERP.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicsERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request,
            CancellationToken ct)
        {
            await _authService.RegisterAsync(request, ct);
            return Ok(new { message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken ct)
        {
            var token = await _authService.LoginAsync(request, ct);

            return Ok(new
            {
                message = "Login successful",
                token
            });
        }
    }
}
