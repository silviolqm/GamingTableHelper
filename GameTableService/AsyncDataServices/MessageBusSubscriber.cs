
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
                QueueDeclareOk queue = await _channel.QueueDeclareAsync();
                _queueName = queue.QueueName;
                await _channel.QueueBindAsync(
                    queue: _queueName,
                    exchange: _configuration["RabbitMQExchange"]!,
                    routingKey: string.Empty
                );

                _connection.ConnectionShutdownAsync += MessageBus_ConnectionShutdown;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection to Message Bus failed: {ex.Message}");
            }
        }

        private async Task MessageBus_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine($"Connection to Message Bus was shut down.");
        }

        protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
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

        public async ValueTask DisposeAsync()
        {
            if (_channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            }
        }
    }
}