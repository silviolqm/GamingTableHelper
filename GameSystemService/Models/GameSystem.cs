using System.ComponentModel.DataAnnotations;

namespace GameSystemService.Models
{
    public class GameSystem
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Publisher { get; set; }
    }
}