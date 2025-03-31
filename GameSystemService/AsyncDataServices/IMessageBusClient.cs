using GameSystemService.Dtos;

namespace GameSystemService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        Task PublishGameSystemEvent(GameSystemEventDto gameSystemPublishedDto);
    }
}