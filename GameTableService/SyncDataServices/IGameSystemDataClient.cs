using GameTableService.Models;

namespace GameTableService.SyncDataServices
{
    public interface IGameSystemDataClient
    {
        IEnumerable<GameSystem> ReturnAllGameSystems();
    }
}