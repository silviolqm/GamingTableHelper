using NotificationService.Dtos;
using NotificationService.Models;

namespace NotificationService.EmailService
{
    public class EmailService : IEmailService
    {
        public (string to, string subject, string body) GenerateTableFullEmail(ApplicationUser user, GameTableFullEventDto gameTable)
        {
            var to = user.Email;
            var subject = $"{user.Username}, your Game Table is ready!";
            var body = $"""
                Dear {user.Username},

                Your Game Table has reached the maximum number of players and is ready for you!
                Table Name: {gameTable.Name}
                Table Description: {gameTable.Description}
                Start Date and Time: {gameTable.StartDateTime.LocalDateTime}

                Thank you for using Gaming Table Helper!
                """;
            return (to, subject, body);
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            // Mock the email sending process
            Console.WriteLine($"Sending email notification to: {to}");
            return Task.Delay(100);
        }
    }
}