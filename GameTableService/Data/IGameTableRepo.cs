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
        void UpdateGameTable(GameTable gameTable);
        void DeleteGameTable(GameTable gameTable);
        IEnumerable<GameSystem> GetGameSystems();
        bool AddPlayerToGameTable(Guid gameTableId, Guid userId);
        bool GameSystemExists(Guid gameSystemId);
        bool ExternalGameSystemExists(Guid externalGameSystemId);
        void CreateGameSystem(GameSystem gameSystem);
        void DeleteGameSystem(GameSystem gameSystem);
        void UpdateGameSystem(GameSystem gameSystem);
    }
}