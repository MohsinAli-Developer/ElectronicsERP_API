//using ElectronicsERP.Data;
//using ElectronicsERP.Models;
//using Microsoft.AspNetCore.Mvc;
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

//        // ✅ REGISTER endpoint
//        [HttpPost("register")]
//        public IActionResult Register([FromBody] RegisterRequest request)
//        {
//            if (request == null || string.IsNullOrWhiteSpace(request.Name) ||
//                string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Role))
//            {
//                return BadRequest(new { message = "Username, Role, and Password are required" });
//            }

//            // Check if username already exists
//            if (_context.Users.Any(u => u.Username == request.Name))
//            {
//                return Conflict(new { message = "Username already exists" });
//            }

//            var role = _context.Roles.FirstOrDefault(r => r.RoleName == request.Role);
//            if (role == null)
//            {
//                return BadRequest(new { message = "Invalid role" });
//            }

//            // ✅ Secure password hashing
//            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

//            var user = new User
//            {
//                Username = request.Name,
//                PasswordHash = hashedPassword,
//                RoleId = role.Id.ToString()
//            };

//            _context.Users.Add(user);
//            _context.SaveChanges();

//            return Ok(new { message = "Registration successful" });
//        }

//        // ✅ LOGIN endpoint with JWT
//        [HttpPost("login")]
//        public IActionResult Login([FromBody] LoginRequest request)
//        {
//            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
//            {
//                return BadRequest(new { message = "Username and password are required" });
//            }

//            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

//            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
//            {
//                return Unauthorized(new { message = "Invalid username or password" });
//            }

//            // ✅ Generate JWT Token
//            var token = GenerateJwtToken(user.Username, user.Role);

//            return Ok(new
//            {
//                message = "Login successful",
//                token
//            });
//        }

//        // ✅ Generate JWT Token
//        private string GenerateJwtToken(string username, string role)
//        {
//            var jwtSettings = _configuration.GetSection("JwtSettings");
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//            {
//                new Claim(ClaimTypes.Name, username),
//                new Claim(ClaimTypes.Role, role)
//            };

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

//    public class RegisterRequest
//    {
//        public string Name { get; set; }
//        public string Role { get; set; }
//        public string Password { get; set; }
//    }

//    public class LoginRequest
//    {
//        public string Username { get; set; }
//        public string Password { get; set; }
//    }
//}

using ElectronicsERP.Data;
using ElectronicsERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace ElectronicsERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ REGISTER endpoint
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Password) || request.Role <= 0)
            {
                return BadRequest(new { message = "Username, Role, and Password are required" });
            }

            // Check if username exists
            if (_context.Users.Any(u => u.Username == request.Name))
            {
                return Conflict(new { message = "Username already exists" });
            }

            // Find role
            var role = _context.Roles.FirstOrDefault(r => r.Id == request.Role);
            if (role == null)
            {
                return BadRequest(new { message = "Invalid role" });
            }

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Name,
                PasswordHash = hashedPassword,
                RoleId = role.Id
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "Registration successful" });
        }

        // ✅ LOGIN endpoint with JWT
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            var user = _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefault(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // ✅ Get all allowed permissions for this role
            var permissions = user.Role.RolePermissions
                .Where(rp => rp.IsAllowed)
                .Select(rp => rp.Permission.Name)
                .ToList();

            // ✅ Generate JWT Token with permissions
            var token = GenerateJwtToken(user.Username, user.Role.Name, permissions);

            return Ok(new
            {
                message = "Login successful",
                token
            });
        }

        // ✅ Generate JWT Token including permissions
        private string GenerateJwtToken(string username, string role, List<string> permissions)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            // Add permission claims
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTOs
    public class RegisterRequest
    {
        public string Name { get; set; }
        public int Role { get; set; }  // change from string → int
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}