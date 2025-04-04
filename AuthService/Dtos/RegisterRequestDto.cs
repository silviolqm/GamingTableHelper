using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(100)]
        public required string UserName { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and a special character.")]
        public required string Password { get; set; }
        
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}