using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory {HostName = "localhost"};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

var exchangeName = "order";

channel.ExchangeDeclare(
    exchange: exchangeName, 
    type: ExchangeType.Direct
);

var queueName = "inventory_queue";
var routingKey = "order.new";

channel.QueueDeclare(
    queue: queueName,
    durable: true,
    exclusive: false, 
    autoDelete: false,
    arguments: null
);

channel.QueueBind(
    queue: queueName, 
    exchange: exchangeName,
    routingKey: routingKey
);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) => 
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($"[x] Received order : {message}");
    Console.WriteLine($"[x] Inventory updating...");
    Console.WriteLine($"[x] Inventory update succeed !");
};

channel.BasicConsume(
    queue: queueName,
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("[x] Press enter to exit...");
Console.ReadLine();