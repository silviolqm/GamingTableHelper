using System.Text;
using System.Text.Json;
using GameSystemService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GameSystemService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IChannel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            SetupMessageBusConnection().Wait();
        }

        async Task SetupMessageBusConnection()
        {
            var factory = new ConnectionFactory() {HostName = _configuration["RabbitMQHost"]!, 
                Port = int.Parse(_configuration["RabbitMQPort"]!)};
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(exchange: _configuration["RabbitMQExchange"]!, type: ExchangeType.Fanout);

                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

                Console.WriteLine($"Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not connect to the MessageBus: {ex.Message}");
            }
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine($"Connection to MessageBus shut down.");
        }

        public async Task PublishNewGameSystem(GameSystemPublishedDto GameSystemPublishedDto)
        {
            var message = JsonSerializer.Serialize(GameSystemPublishedDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine($"Sending message through MessageBus...");
                await SendMessage(message);
            }
            else
            {
                Console.WriteLine($"Failed to send message. Connection to MessageBus closed.");
            }
        }

        private async Task SendMessage(string message)
        {
            byte[] messageBody = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(
                exchange: _configuration["RabbitMQExchange"]!,
                routingKey: string.Empty,
                body: messageBody);
            Console.WriteLine($"Message Sent: {message}");
        }

        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("MessageBus disposed.");
            if (_channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            }
        }
    }
}