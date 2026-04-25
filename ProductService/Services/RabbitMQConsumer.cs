using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ProductService.Data;
using ProductService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumer(IServiceProvider serviceProvider)
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
            _channel.QueueDeclare(queue: "order-cancelled", durable: true, exclusive: false, autoDelete: false, arguments: null);
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
                var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
                var product = await context.Products.FirstOrDefaultAsync(p => p.Id == orderEvent.ProductId);
                if (product != null)
                {
                    product.Stock -= orderEvent.Quantity;
                    await context.SaveChangesAsync();
                }
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            _channel.BasicConsume(queue: "order-created", autoAck: false, consumer: consumer);

            var cancelledConsumer = new EventingBasicConsumer(_channel);
            cancelledConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var cancelledEvent = JsonSerializer.Deserialize<OrderCancelledEvent>(message);
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
                var product = await context.Products.FirstOrDefaultAsync(p => p.Id == cancelledEvent.ProductId);
                if (product != null)
                {
                    product.Stock += cancelledEvent.Quantity;
                    await context.SaveChangesAsync();
                }
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            _channel.BasicConsume(queue: "order-cancelled", autoAck: false, consumer: cancelledConsumer);
            await Task.CompletedTask;
        }

        public class OrderCreatedEvent
        {
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }

        public class OrderCancelledEvent
        {
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
