namespace NotificationService.AsyncDataServices
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string message);
    }
}