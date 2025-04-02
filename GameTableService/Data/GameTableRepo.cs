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

        public void EditGameTable(GameTable gameTable)
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

        public IEnumerable<GameTable> GetAllGameTables()
        {
            return _context.GameTables.ToList();
        }

        public GameTable GetGameTableById(Guid id)
        {
            return _context.GameTables.FirstOrDefault(p => p.Id == id)!;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}