namespace AuthService.Dtos
{
    public class UserEventDto
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Event { get; set; }
    }
}