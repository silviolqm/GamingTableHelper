using System.ComponentModel.DataAnnotations;

namespace GameTableService.Dtos
{
    public class GameTableUpdateDto
    {
        public Guid OwnerUserId { get; set; }
        [Required]
        public Guid GameSystemId { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public ICollection<Guid> Players { get; set; } = new List<Guid>();
        [Required]
        public DateTimeOffset StartDateTime { get; set; }
    }
}