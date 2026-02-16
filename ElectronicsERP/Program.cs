//using ElectronicsERP.Data;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;

//namespace ElectronicsERP
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // ✅ Database connection
//            // Mohsin Update
//            // awais toop updated
//            builder.Services.AddDbContext<ApplicationDbContext>(options =>
//                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//            // ✅ JWT Authentication
//            var jwtSettings = builder.Configuration.GetSection("JwtSettings");

//            builder.Services.AddAuthentication(options =>
//            {
//                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//            })
//            .AddJwtBearer(options =>
//            {
//                options.TokenValidationParameters = new TokenValidationParameters
//                {
//                    ValidateIssuer = true,
//                    ValidateAudience = true,
//                    ValidateLifetime = true,
//                    ValidateIssuerSigningKey = true,
//                    ValidIssuer = jwtSettings["Issuer"],
//                    ValidAudience = jwtSettings["Audience"],
//                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
//                };
//            });

//            builder.Services.AddAuthorization();

//            builder.Services.AddControllers();



//            // Add services to the container.
//            builder.Services.AddControllers();
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();


//            // Add CORS policy BEFORE builder.Build()
//            //builder.Services.AddCors(options =>
//            //{
//            //    options.AddPolicy("AllowReactApp",
//            //        policy =>
//            //        {
//            //            policy.WithOrigins("http://localhost:5173", "http://localhost:8083") // ✅ Allow both dev and nginx
//            //                  .AllowAnyHeader()
//            //                  .AllowAnyMethod()
//            //                  .AllowCredentials();
//            //        });
//            //});
//            builder.Services.AddCors(options =>
//            {
//                options.AddPolicy("AllowAll", policy =>
//                {
//                    policy.AllowAnyOrigin()
//                          .AllowAnyMethod()
//                          .AllowAnyHeader();
//                });
//            });


//            var app = builder.Build();

//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            // 🟢 CORS MUST come before Authentication
//            app.UseCors("AllowReactApp");

//            app.UseHttpsRedirection();

//            app.UseAuthentication();

//            app.UseAuthorization();

//            app.MapControllers();

//            app.Run();
//        }
//    }
//}


using ElectronicsERP.Data;
using ElectronicsERP.Repositories.Implementations;
using ElectronicsERP.Repositories.Interfaces;
using ElectronicsERP.Services.Implementations;
using ElectronicsERP.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ElectronicsERP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Listen on all network interfaces (IMPORTANT!)j
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5010);  // same port as your Flutter app
            });

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Correct CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")                         
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            //builder.Services.AddCors(options =>
            //            //{
            //            //    options.AddPolicy("AllowReactApp",
            //            //        policy =>
            //            //        {
            //            //            policy.WithOrigins("http://localhost:5173", "http://localhost:8083") // ✅ Allow both dev and nginx
            //            //                  .AllowAnyHeader()
            //            //                  .AllowAnyMethod()
            //            //                  .AllowCredentials();
            //            //        });
            //            //});
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use correct policy
            app.UseCors("AllowReactApp");
            //app.UseHsts();

            //app.UseHttpsRedirection();
            app.UseMiddleware<BasicAuthMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}