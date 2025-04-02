using System.ComponentModel.DataAnnotations;

namespace GameTableService.Models
{
    public class GameTable
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
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
        public DateTimeOffset startDateTime { get; set; }
        public GameSystem GameSystem { get; set; } = null!; //Navigation property for the model builder in AppDbContext
    }
}