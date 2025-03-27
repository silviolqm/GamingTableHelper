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
        List<User> placeholderUsers = new() 
        {
            new("admin", "123456e7"),
            new("user01", "123456e7")
        };

        public string? GenerateAuthToken(LoginRequestDto loginRequestDto)
        {
            var user = placeholderUsers.FirstOrDefault(u => u.UserName == loginRequestDto.Username && u.PasswordHash == loginRequestDto.Password);
            if (user is null)
            {
                return null;
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtExtensions.SECURITY_KEY));
            var signingCreds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddMinutes(15);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            };

            var securityToken = new JwtSecurityToken
            (
                issuer: JwtExtensions.VALID_ISSUER,
                expires: expiration,
                claims: claims,
                signingCredentials: signingCreds
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}