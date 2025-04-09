using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IChannel _channel;
        private string _queueUserEvents;
        private string _queueTableFullEvents;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
        }

        async Task SetupMessageBusConnection()
        {
            var factory = new ConnectionFactory() {HostName = _configuration["RabbitMQHost"]!, 
                Port = int.Parse(_configuration["RabbitMQPort"]!)};
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                //User queue
                await _channel.ExchangeDeclareAsync(exchange: _configuration["RabbitMQExchangeAppUser"]!, type: ExchangeType.Fanout);
                QueueDeclareOk queueUser = await _channel.QueueDeclareAsync();
                _queueUserEvents = queueUser.QueueName;
                await _channel.QueueBindAsync(
                    queue: _queueUserEvents,
                    exchange: _configuration["RabbitMQExchangeAppUser"]!,
                    routingKey: string.Empty
                );
                
                //Game Table Full queue
                await _channel.ExchangeDeclareAsync(exchange: _configuration["RabbitMQExchangeGameTable"]!, type: ExchangeType.Fanout);
                QueueDeclareOk queueGameTableFull = await _channel.QueueDeclareAsync();
                _queueTableFullEvents = queueGameTableFull.QueueName;
                await _channel.QueueBindAsync(
                    queue: _queueTableFullEvents,
                    exchange: _configuration["RabbitMQExchangeGameTable"]!,
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

        protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                await SetupMessageBusConnection();
            }

            stoppingToken.ThrowIfCancellationRequested();

            //User consumer
            var consumerUser = new AsyncEventingBasicConsumer(_channel);
            consumerUser.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            //Game Table Full consumer
            var consumerGameTableFull = new AsyncEventingBasicConsumer(_channel);
            consumerGameTableFull.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };
            
            await _channel.BasicConsumeAsync(queue: _queueUserEvents, autoAck: false, consumer: consumerUser);
            await _channel.BasicConsumeAsync(queue: _queueTableFullEvents, autoAck: false, consumer: consumerGameTableFull);

            return Task.CompletedTask;
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