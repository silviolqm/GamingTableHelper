
using System.Runtime.CompilerServices;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GameTableService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IChannel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
        }

        protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                await SetupMessageBusConnection();
            }

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };
            
            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        async Task SetupMessageBusConnection()
        {
            var factory = new ConnectionFactory() {HostName = _configuration["RabbitMQHost"]!, 
                Port = int.Parse(_configuration["RabbitMQPort"]!)};
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(exchange: _configuration["RabbitMQExchangeGameSys"]!, type: ExchangeType.Fanout);
                QueueDeclareOk queue = await _channel.QueueDeclareAsync();
                _queueName = queue.QueueName;
                await _channel.QueueBindAsync(
                    queue: _queueName,
                    exchange: _configuration["RabbitMQExchangeGameSys"]!,
                    routingKey: string.Empty
                );

                _connection.ConnectionShutdownAsync += MessageBus_ConnectionShutdown;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection to Message Bus failed: {ex.Message}");
                Console.WriteLine("RabbitMQ setup failed.");
                Console.WriteLine($"Error: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        private async Task MessageBus_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine($"Connection to Message Bus was shut down.");
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