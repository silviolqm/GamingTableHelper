using GameTableService.Dtos;

namespace GameTableService.AsyncDataServices
{
    public interface IMessageBusPublisher
    {
        Task PublishGameTableFullEvent(GameTableFullEventDto gameTableFullEventDto);
    }
}