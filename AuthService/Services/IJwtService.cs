using AuthService.Dtos;
using AuthService.Models;

namespace AuthService.Services
{
    public interface IJwtService
    {
        string GenerateAuthToken(ApplicationUser user);
    }
}