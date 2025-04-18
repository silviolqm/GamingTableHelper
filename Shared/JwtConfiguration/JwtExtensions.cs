using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Shared.JwtConfiguration
{
    public static class JwtExtensions
    {
        // The security key should be stored safely in the production environment.
        public const string SECURITY_KEY = "super-secret-keysuper-secret-key";
        public const string VALID_ISSUER = "GamingTableHelper";
        public const string VALID_AUDIENCE = "https://localhost:7003";

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

        public static void AddSwaggerGenWithJWT(this IServiceCollection services, string serviceName, string description)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = $"{serviceName} API", Version = "v1", Description = description });
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter the JWT Token",
                    Name = "JWT Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[] { } }
                });
            });
        }
    }
}