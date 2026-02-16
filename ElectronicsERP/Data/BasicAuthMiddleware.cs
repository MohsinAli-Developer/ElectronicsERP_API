using System.Net.Http.Headers;
using System.Text;

namespace ElectronicsERP.Data
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration config)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                await Unauthorized(context);
                return;
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(
                    context.Request.Headers["Authorization"]
                );

                if (!authHeader.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                {
                    await Unauthorized(context);
                    return;
                }

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                if (credentials.Length != 2)
                {
                    await Unauthorized(context);
                    return;
                }

                var username = credentials[0];
                var password = credentials[1];

                var validUsername = config["BasicAuth:Username"];
                var validPassword = config["BasicAuth:Password"];

                if (username != validUsername || password != validPassword)
                {
                    await Unauthorized(context);
                    return;
                }

                // Auth success → continue
                await _next(context);
            }
            catch
            {
                await Unauthorized(context);
            }
        }

        private static async Task Unauthorized(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            await context.Response.WriteAsync("Unauthorized");
        }
    }

}

