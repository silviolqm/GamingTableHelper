namespace GameSystemService.Dtos
{
    public class GameSystemPublishedDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Event { get; set; }
    }
}