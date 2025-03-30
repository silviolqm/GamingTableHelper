using GameSystemService.Models;

namespace GameSystemService.Data
{
    public interface IGameSystemRepo
    {
        bool SaveChanges();
        IEnumerable<GameSystem> GetAllGameSystems();
        GameSystem GetGameSystemById(Guid id);
        void CreateGameSystem(GameSystem gameSystem);
        void EditGameSystem(GameSystem gameSystem);
        void DeleteGameSystem(GameSystem gameSystem);
    }
}