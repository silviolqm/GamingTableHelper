using System.Text.Json;
using AutoMapper;
using GameTableService.Data;
using GameTableService.Dtos;
using GameTableService.Models;

namespace GameTableService.AsyncDataServices
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.GameSystemCreated:
                    AddGameSystem(message);
                    break;
                case EventType.GameSystemDeleted:
                    DeleteGameSystem(message);
                    break;
                case EventType.GameSystemUpdated:
                    UpdateGameSystem(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            if (eventType == null)
            {
                return EventType.Undertermined;
            }

            switch(eventType.Event)
            {
                case "GameSystem_Created":
                    return EventType.GameSystemCreated;
                case "GameSystem_Deleted":
                    return EventType.GameSystemDeleted;
                case "GameSystem_Updated":
                    return EventType.GameSystemUpdated;
                default:
                    return EventType.Undertermined;
            }
        }

        private void AddGameSystem(string message)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IGameTableRepo>();

                var gameSysEventDto = JsonSerializer.Deserialize<GameSystemEventDto>(message);

                try
                {
                    var gameSystem = _mapper.Map<GameSystem>(gameSysEventDto);
                    if (!repo.ExternalGameSystemExists(gameSystem.ExternalId))
                    {
                        repo.CreateGameSystem(gameSystem);
                        repo.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to add Game System to database: {ex.Message}");
                }
            }
        }

        private void DeleteGameSystem(string message)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IGameTableRepo>();

                var gameSysEventDto = JsonSerializer.Deserialize<GameSystemEventDto>(message);

                try
                {
                    var gameSystem = _mapper.Map<GameSystem>(gameSysEventDto);
                    if (repo.ExternalGameSystemExists(gameSystem.ExternalId))
                    {
                        repo.DeleteGameSystem(gameSystem);
                        repo.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete Game System in database: {ex.Message}");
                }
            }
        }

        private void UpdateGameSystem(string message)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IGameTableRepo>();

                var gameSysEventDto = JsonSerializer.Deserialize<GameSystemEventDto>(message);

                try
                {
                    var gameSystem = _mapper.Map<GameSystem>(gameSysEventDto);
                    if (repo.ExternalGameSystemExists(gameSystem.ExternalId))
                    {
                        repo.UpdateGameSystem(gameSystem);
                        repo.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update Game System in database: {ex.Message}");
                }
            }
        }
    }

    enum EventType
    {
        GameSystemCreated,
        GameSystemDeleted,
        GameSystemUpdated,
        Undertermined
    }
}