using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace OrderService.Services
{
    public class RabbitMQPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQPublisher()
        {
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
            
            Console.WriteLine("RabbitMQ Publisher initialized. Queues: order-created, order-cancelled");
        }

        public void PublishOrderCreated(int orderId, int productId, int quantity, decimal totalPrice)
        {
            var orderEvent = new
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                TotalPrice = totalPrice
            };
            var message = JsonSerializer.Serialize(orderEvent);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "order-created", basicProperties: null, body: body);
            Console.WriteLine($"OrderCreated event published for OrderId: {orderId}");
        }

        public void PublishOrderCancelled(int orderId, int productId, int quantity)
        {
            var cancelledEvent = new
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity
            };
            var message = JsonSerializer.Serialize(cancelledEvent);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "order-cancelled", basicProperties: null, body: body);
            Console.WriteLine($"OrderCancelled event published for OrderId: {orderId}, ProductId: {productId}, Quantity: {quantity}");
        }
    }
}
