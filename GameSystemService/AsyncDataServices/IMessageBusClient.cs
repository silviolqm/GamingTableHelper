using GameSystemService.Dtos;

namespace GameSystemService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        public interface IMessageBusClient
        {
            Task PublishNewGameSystem(GameSystemPublishedDto gameSystemPublishedDto);
        }
    }
}