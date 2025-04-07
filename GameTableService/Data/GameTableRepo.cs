using GameTableService.Models;

namespace GameTableService.Data
{
    public class GameTableRepo : IGameTableRepo
    {
        private readonly AppDbContext _context;

        public GameTableRepo(AppDbContext context)
        {
            _context = context;
        }

        public bool AddPlayerToGameTable(Guid gameTableId, Guid userId)
        {
            var gameTable = _context.GameTables.FirstOrDefault(g => g.Id == gameTableId);
            if (gameTable == null)
            {
                throw new ArgumentNullException(nameof(gameTable));
            }
            if (!gameTable.Players.Contains(userId))
            {
                gameTable.Players.Add(userId);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateGameSystem(GameSystem gameSystem)
        {
            if (gameSystem == null)
            {
                throw new ArgumentNullException(nameof(gameSystem));
            }
            _context.GameSystems.Add(gameSystem);
        }

        public void CreateGameTable(GameTable gameTable)
        {
            if (gameTable == null)
            {
                throw new ArgumentNullException(nameof(gameTable));
            }
            _context.GameTables.Add(gameTable);
        }

        public void DeleteGameTable(GameTable gameTable)
        {
            if (gameTable == null)
            {
                throw new ArgumentNullException(nameof(gameTable));
            }
            _context.GameTables.Remove(gameTable);
        }

        public void UpdateGameTable(GameTable gameTable)
        {
            if (gameTable == null)
            {
                throw new ArgumentNullException(nameof(gameTable));
            }

            var existingGameTable = _context.GameTables.FirstOrDefault(gs => gs.Id == gameTable.Id);
            if (existingGameTable == null)
            {
                throw new KeyNotFoundException($"GameTable with ID {gameTable.Id} not found.");
            }

            _context.Entry(existingGameTable).CurrentValues.SetValues(gameTable);
        }

        public bool ExternalGameSystemExists(Guid externalGameSystemId)
        {
            return _context.GameSystems.Any(p => p.ExternalId == externalGameSystemId);
        }

        public bool GameSystemExists(Guid gameSystemId)
        {
            if (_context.GameSystems.FirstOrDefault(gs => gs.Id == gameSystemId) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public IEnumerable<GameTable> GetAllGameTables()
        {
            return _context.GameTables.ToList();
        }

        public IEnumerable<GameTable> GetAllGameTablesByGameSystem(Guid? gameSystemId)
        {
            return _context.GameTables.Where(g => g.GameSystemId == gameSystemId);
        }

        public IEnumerable<GameSystem> GetGameSystems()
        {
            return _context.GameSystems.ToList();
        }

        public GameTable GetGameTableById(Guid id)
        {
            return _context.GameTables.FirstOrDefault(p => p.Id == id)!;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public void DeleteGameSystem(GameSystem gameSystem)
        {
            if (gameSystem == null)
            {
                throw new ArgumentNullException(nameof(gameSystem));
            }
            _context.GameSystems.Remove(gameSystem);
        }

        public void UpdateGameSystem(GameSystem gameSystem)
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
    }
}