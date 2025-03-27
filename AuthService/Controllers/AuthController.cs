using AuthService.Dtos;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public AuthController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost]
        public ActionResult<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var token = _jwtService.GenerateAuthToken(loginRequestDto);

            if (token is null)
            {
                return Unauthorized();
            }

            var response = new LoginResponseDto();
            response.Token = token;
            response.UserName = loginRequestDto.Username;

            return Ok(response);
        }
    }
}