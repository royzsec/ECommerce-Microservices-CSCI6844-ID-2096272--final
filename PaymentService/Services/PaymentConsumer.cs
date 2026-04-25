using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using PaymentService.Data;
using PaymentService.Models;

namespace PaymentService.Services
{
    public class PaymentConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public PaymentConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "order-created", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
                var payment = new Payment
                {
                    OrderId = orderEvent.OrderId,
                    Amount = orderEvent.TotalPrice,
                    Status = "Completed",
                    PaymentDate = DateTime.UtcNow
                };
                await context.Payments.AddAsync(payment);
                await context.SaveChangesAsync();
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            _channel.BasicConsume(queue: "order-created", autoAck: false, consumer: consumer);
            await Task.CompletedTask;
        }

        public class OrderCreatedEvent
        {
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}
