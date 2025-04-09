namespace NotificationService.AsyncDataServices
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}