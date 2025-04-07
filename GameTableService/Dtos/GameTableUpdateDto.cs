using System.ComponentModel.DataAnnotations;

namespace GameTableService.Dtos
{
    public class GameTableUpdateDto
    {
        [Required]
        public Guid GameSystemId { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        [Required]
        public DateTimeOffset StartDateTime { get; set; }
    }
}