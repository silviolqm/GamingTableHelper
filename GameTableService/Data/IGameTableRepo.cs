using GameTableService.Models;

namespace GameTableService.Data
{
    public interface IGameTableRepo
    {
        bool SaveChanges();
        IEnumerable<GameTable> GetAllGameTables();
        GameTable GetGameTableById(Guid id);
        void CreateGameTable(GameTable gameTable);
        void EditGameTable(GameTable gameTable);
        void DeleteGameTable(GameTable gameTable);
    }
}