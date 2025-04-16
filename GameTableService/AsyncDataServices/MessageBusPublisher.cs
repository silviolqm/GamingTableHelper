using System.Text;
using System.Text.Json;
using GameTableService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GameTableService.AsyncDataServices
{
    public class MessageBusPublisher : IMessageBusPublisher
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IChannel _channel;

        public MessageBusPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishGameTableFullEvent(GameTableFullEventDto gameTableFullEventDto)
        {
            if (_channel == null)
            {
                await SetupMessageBusConnection();
            }

            var message = JsonSerializer.Serialize(gameTableFullEventDto);
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

        async Task SetupMessageBusConnection()
        {
            var factory = new ConnectionFactory() {HostName = _configuration["RabbitMQHost"]!, 
                Port = int.Parse(_configuration["RabbitMQPort"]!)};
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(exchange: _configuration["RabbitMQExchangeGameTable"]!, type: ExchangeType.Fanout);

                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

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
            await _channel.BasicPublishAsync(
                exchange: _configuration["RabbitMQExchangeGameTable"]!,
                routingKey: string.Empty,
                body: messageBody);
            Console.WriteLine($"Message Sent: {message}");
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine($"Connection to MessageBus shut down.");
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