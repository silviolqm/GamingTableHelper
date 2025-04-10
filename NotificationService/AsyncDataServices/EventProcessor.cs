using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using NotificationService.Data;
using NotificationService.Dtos;
using NotificationService.EmailService;
using NotificationService.Models;

namespace NotificationService.AsyncDataServices
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
            _scopeFactory = scopeFactory;
        }

        public async Task ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.UserCreated:
                    AddUser(message);
                    break;
                case EventType.UserDeleted:
                    DeleteUser(message);
                    break;
                case EventType.UserUpdated:
                    UpdateUser(message);
                    break;
                case EventType.GameTableFull:
                    await SendNotificationTableFull(message);
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
                case "User_Created":
                    return EventType.UserCreated;
                case "User_Deleted":
                    return EventType.UserDeleted;
                case "User_Updated":
                    return EventType.UserUpdated;
                case "GameTable_Full":
                    return EventType.GameTableFull;
                default:
                    return EventType.Undertermined;
            }
        }

        private void AddUser(string message)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<INotificationRepo>();

                var userEventDto = JsonSerializer.Deserialize<UserEventDto>(message);

                try
                {
                    var user = _mapper.Map<ApplicationUser>(userEventDto);
                    if (!repo.ExternalUserExists(user.ExternalId))
                    {
                        repo.CreateUser(user);
                        repo.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to add User to database: {ex.Message}");
                }
            }
        }

        private void DeleteUser(string message)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<INotificationRepo>();

                var userEventDto = JsonSerializer.Deserialize<UserEventDto>(message);

                try
                {
                    var user = _mapper.Map<ApplicationUser>(userEventDto);
                    if (repo.ExternalUserExists(user.ExternalId))
                    {
                        repo.DeleteUser(user);
                        repo.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete User in database: {ex.Message}");
                }
            }
        }

        private void UpdateUser(string message)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<INotificationRepo>();

                var userEventDto = JsonSerializer.Deserialize<UserEventDto>(message);

                try
                {
                    var user = _mapper.Map<ApplicationUser>(userEventDto);
                    if (repo.ExternalUserExists(user.ExternalId))
                    {
                        repo.UpdateUser(user);
                        repo.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update User in database: {ex.Message}");
                }
            }
        }

        private async Task SendNotificationTableFull(string message)
        {
            var gameTableFullEventDto = JsonSerializer.Deserialize<GameTableFullEventDto>(message);
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<INotificationRepo>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            foreach (var playerId in gameTableFullEventDto!.Players)
            {
                var user = repo.GetByUserId(playerId);
                var email = emailService.GenerateTableFullEmail(user, gameTableFullEventDto);
                await emailService.SendEmailAsync(email.to, email.subject, email.body);
            }
            Console.WriteLine($"All players notified by email that the game table is ready!");
        }
    }

    enum EventType
    {
        UserCreated,
        UserDeleted,
        UserUpdated,
        GameTableFull,
        Undertermined
    }
}