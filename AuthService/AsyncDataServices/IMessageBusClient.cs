using AuthService.Dtos;

namespace AuthService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        Task PublishUserEvent(UserEventDto userEventDto);
    }
}