namespace GameTableService.Dtos
{
    public class GameTableReadDto
    {
        public Guid Id { get; set; }
        public Guid OwnerUserId { get; set; }
        public Guid GameSystemId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public ICollection<Guid> Players { get; set; } = new List<Guid>();
        public DateTimeOffset StartDateTime { get; set; }
    }
}