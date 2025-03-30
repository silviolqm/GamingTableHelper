using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(100, ErrorMessage = "User Name maximum length is 100 characters.")]
        public required string UserName { get; set; }
        
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and a special character.")]
        public required string Password { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }
    }
}