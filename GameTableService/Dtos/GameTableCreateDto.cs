using System.ComponentModel.DataAnnotations;

namespace GameTableService.Dtos
{
    [ValidPlayerRange]
    public class GameTableCreateDto
    {
        [Required]
        public Guid GameSystemId { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        [Required]
        [StringLength(256)]
        public required string Description { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int MinPlayers { get; set; }
        [Range(2, int.MaxValue)]
        public int MaxPlayers { get; set; }
        [Required]
        public DateTimeOffset StartDateTime { get; set; }
    }
}