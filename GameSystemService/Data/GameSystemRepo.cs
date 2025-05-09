using AutoMapper;
using GameSystemService.Models;

namespace GameSystemService.Data
{
    public class GameSystemRepo : IGameSystemRepo
    {
        private readonly AppDbContext _context;

        public GameSystemRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreateGameSystem(GameSystem gameSystem)
        {
            if (gameSystem == null)
            {
                throw new ArgumentNullException(nameof(gameSystem));
            }
            _context.GameSystems.Add(gameSystem);
        }

        public void DeleteGameSystem(GameSystem gameSystem)
        {
            if (gameSystem == null)
            {
                throw new ArgumentNullException(nameof(gameSystem));
            }
            _context.GameSystems.Remove(gameSystem);
        }

        public void EditGameSystem(GameSystem gameSystem)
        {
            if (gameSystem == null)
            {
                throw new ArgumentNullException(nameof(gameSystem));
            }

            var existingGameSystem = _context.GameSystems.FirstOrDefault(gs => gs.Id == gameSystem.Id);
            if (existingGameSystem == null)
            {
                throw new KeyNotFoundException($"GameSystem with ID {gameSystem.Id} not found.");
            }

            _context.Entry(existingGameSystem).CurrentValues.SetValues(gameSystem);
        }

        public IEnumerable<GameSystem> GetAllGameSystems()
        {
            return _context.GameSystems.ToList();
        }

        public GameSystem GetGameSystemById(Guid id)
        {
            return _context.GameSystems.FirstOrDefault(p => p.Id == id)!;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}