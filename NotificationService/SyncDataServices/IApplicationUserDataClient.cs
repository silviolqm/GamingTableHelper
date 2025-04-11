using NotificationService.Models;

namespace NotificationService.SyncDataServices
{
    public interface IApplicationUserDataClient
    {
        IEnumerable<ApplicationUser> ReturnAllUsers();
    }
}