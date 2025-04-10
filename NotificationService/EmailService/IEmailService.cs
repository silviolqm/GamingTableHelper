using NotificationService.Dtos;
using NotificationService.Models;

namespace NotificationService.EmailService
{
    public interface IEmailService
    {
        (string to, string subject, string body) GenerateTableFullEmail(ApplicationUser user, GameTableFullEventDto gameTable);
        Task SendEmailAsync(string to, string subject, string body);
    }
}