using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    public class User : IdentityUser
    {
        public User(string username, string password)
        {
            UserName = username;
            PasswordHash = password;
        }
    }
}