using GameTableService.Models;

namespace GameTableService.Data
{
    public interface IGameTableRepo
    {
        bool SaveChanges();
        IEnumerable<GameTable> GetAllGameTables();
        IEnumerable<GameTable> GetAllGameTablesByGameSystem(Guid? gameSystemId);
        GameTable GetGameTableById(Guid id);
        void CreateGameTable(GameTable gameTable);
        void EditGameTable(GameTable gameTable);
        void DeleteGameTable(GameTable gameTable);
        IEnumerable<GameSystem> GetGameSystems();
        bool AddPlayerToGameTable(Guid gameTableId, Guid userId);
    }
}