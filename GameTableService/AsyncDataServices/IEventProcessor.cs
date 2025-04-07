namespace GameTableService.AsyncDataServices
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}