using System.ComponentModel.DataAnnotations;

namespace GameTableService.Models
{
    public class GameSystem
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ExternalId { get; set; }
        [Required]
        public required string Name { get; set; }
        public ICollection<GameTable> GameTables { get; set; } = new List<GameTable>(); //Navigation property for the model builder in AppDbContext
    }
}