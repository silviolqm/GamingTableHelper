namespace NotificationService.Models
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public Guid ExternalId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}