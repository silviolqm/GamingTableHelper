using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Dtos;
using AuthService.Models;
using Microsoft.IdentityModel.Tokens;
using Shared.JwtConfiguration;

namespace AuthService.Services
{
    public class JwtService : IJwtService
    {
        public string GenerateAuthToken(ApplicationUser user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtExtensions.SECURITY_KEY));
            var signingCreds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddMinutes(15);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var securityToken = new JwtSecurityToken
            (
                issuer: JwtExtensions.VALID_ISSUER,
                audience: JwtExtensions.VALID_AUDIENCE,
                expires: expiration,
                claims: claims,
                signingCredentials: signingCreds
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}