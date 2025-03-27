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
        public const string VALID_ISSUER = "GamingTableHelper";
        public const string VALID_AUDIENCE = "https://localhost:7000";

        public static void AddJwtAuthentication (this IServiceCollection services)
        {
            services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                options.UseSecurityTokenValidators = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = VALID_ISSUER,
                    ValidateAudience = true,
                    ValidAudience = VALID_AUDIENCE,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECURITY_KEY))
                };
            });
            services.AddAuthorization();
        }
    }
}