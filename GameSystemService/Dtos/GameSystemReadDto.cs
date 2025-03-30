using System.ComponentModel.DataAnnotations;

namespace GameSystemService.Dtos
{
    public class GameSystemReadDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Publisher { get; set; }
    }
}