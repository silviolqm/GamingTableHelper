using AuthService.Dtos;

namespace AuthService.Services
{
    public interface IJwtService
    {
        string? GenerateAuthToken(LoginRequestDto loginRequestDto);
    }
}