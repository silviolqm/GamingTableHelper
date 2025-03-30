using AuthService.Dtos;
using AuthService.Models;
using AuthService.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IJwtService jwtService, UserManager<ApplicationUser> userManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);
            if (user is null ||
                new PasswordHasher<ApplicationUser>()
                .VerifyHashedPassword(user, user.PasswordHash!, loginRequest.Password) == PasswordVerificationResult.Failed)
            {
                return Problem(
                    type: "Unauthorized",
                    title: "Login Failed",
                    detail: "User name and/or password don't match.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }
            
            var token = _jwtService.GenerateAuthToken(user);

            var response = new LoginResponseDto {UserName = loginRequest.Username, Token = token};

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto registerRequest)
        {
            if(_userManager.Users.Any(u => u.Email == registerRequest.Email))
            {
                return Problem(
                    type: "Bad Request",
                    title: "Invalid Email",
                    detail: "An account with this email already exists.",
                    statusCode: StatusCodes.Status400BadRequest);
            }
            if(_userManager.Users.Any(u => u.UserName == registerRequest.UserName))
            {
                return Problem(
                    type: "Bad Request",
                    title: "Invalid User Name",
                    detail: "User name is already taken.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            ApplicationUser newUser = new ApplicationUser();
            newUser.UserName = registerRequest.UserName;
            newUser.Email = registerRequest.Email;
            newUser.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(newUser, registerRequest.Password);

            var userCreationResult = await _userManager.CreateAsync(newUser);

            if (userCreationResult.Succeeded)
            {
                var token = _jwtService.GenerateAuthToken(newUser);
                RegisterResponseDto response = new RegisterResponseDto{
                    Email = newUser.Email,
                    Id = newUser.Id,
                    UserName = newUser.UserName,
                    Token = token
                };
                return Ok(response);
            }
            else
            {
                return Problem(
                    type: "Bad Request",
                    title: "User Creation Failed",
                    detail: "There was a problem while trying to create the user.",
                    statusCode: StatusCodes.Status400BadRequest);
            }
        }
    }
}