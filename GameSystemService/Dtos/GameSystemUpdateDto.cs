using System.ComponentModel.DataAnnotations;

namespace GameSystemService.Dtos
{
    public class GameSystemUpdateDto
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Publisher { get; set; }
    }
}