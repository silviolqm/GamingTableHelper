using NotificationService.Models;

namespace NotificationService.Data
{
    public class NotificationRepo : INotificationRepo
    {
        private readonly AppDbContext _context;

        public NotificationRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreateUser(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.Users.Add(user);
        }

        public void DeleteUser(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.Users.Remove(user);
        }

        public bool ExternalUserExists(Guid externalUserId)
        {
            return _context.Users.Any(p => p.ExternalId == externalUserId);
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public string GetEmailByUserId(Guid id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Email;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public void UpdateUser(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var existingUser = _context.Users.FirstOrDefault(gs => gs.Id == user.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {user.Id} not found.");
            }

            _context.Entry(existingUser).CurrentValues.SetValues(user);
        }

        public bool UserExists(Guid userId)
        {
            if (_context.Users.FirstOrDefault(gs => gs.Id == userId) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}