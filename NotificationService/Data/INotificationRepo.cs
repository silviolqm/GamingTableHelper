using NotificationService.Models;

namespace NotificationService.Data
{
    public interface INotificationRepo
    {
        bool SaveChanges();
        IEnumerable<ApplicationUser> GetAllUsers();
        bool UserExists(Guid userId);
        bool ExternalUserExists(Guid externalUserId);
        void CreateUser(ApplicationUser user);
        void DeleteUser(ApplicationUser user);
        void UpdateUser(ApplicationUser user);
        string GetEmailByUserId(Guid id);
    }
}