using System.Text;
using System.Text.Json;
using AuthService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AuthService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishUserEvent(UserEventDto userEventDto)
        {
            if (_channel == null)
            {
                await SetupMessageBusConnection();
            }

            var message = JsonSerializer.Serialize(userEventDto);
            if (_connection != null && _channel != null && _connection.IsOpen)
            {
                Console.WriteLine($"Sending message through MessageBus...");
                await SendMessage(message);
            }
            else
            {
                Console.WriteLine($"Failed to send message to MessageBus.");
                return;
            }
        }

        async Task SetupMessageBusConnection()
        {
            var factory = new ConnectionFactory() {HostName = _configuration["RabbitMQHost"]!, 
                Port = int.Parse(_configuration["RabbitMQPort"]!)};
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(exchange: _configuration["RabbitMQExchangeAppUser"]!, type: ExchangeType.Fanout);

                Console.WriteLine($"Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not connect to the MessageBus: {ex.Message}");
            }
        }

        private async Task SendMessage(string message)
        {
            byte[] messageBody = Encoding.UTF8.GetBytes(message);
            await _channel!.BasicPublishAsync(
                exchange: _configuration["RabbitMQExchangeAppUser"]!,
                routingKey: string.Empty,
                body: messageBody);
            Console.WriteLine($"Message Sent: {message}");
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
            }
            if (_connection != null && _connection.IsOpen)
            {
                await _connection.CloseAsync();
            }
        }
    }
}