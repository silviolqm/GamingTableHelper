namespace NotificationService.Dtos
{
    public class GameTableFullEventDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public ICollection<Guid> Players { get; set; } = new List<Guid>();
        public DateTimeOffset StartDateTime { get; set; }
        public required string Event { get; set; }
    }
}