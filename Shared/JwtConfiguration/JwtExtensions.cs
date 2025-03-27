using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Shared.JwtConfiguration
{
    public static class JwtExtensions
    {
        // The security key should be stored safely in the production environment.
        public const string SECURITY_KEY = "super-secret-keysuper-secret-key";
        public const string VALID_ISSUER = "https://localhost:7000";
        public const string VALID_AUDIENCE = "https://localhost:7002";

        public static void AddJwtAuthentication (this IServiceCollection services)
        {
            services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = VALID_ISSUER,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECURITY_KEY))
                };
            });
            services.AddAuthorization();
        }
    }
}