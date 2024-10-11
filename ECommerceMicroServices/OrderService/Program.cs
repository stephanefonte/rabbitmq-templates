using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost"};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

var exchangeName = "order";

channel.ExchangeDeclare(
    exchange: exchangeName, 
    type: ExchangeType.Direct
);

var routingKey = "order.new";
var message = args.Length > 0 ? args[0] : "1 T-shirt";
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(
    exchange: exchangeName, 
    routingKey: routingKey, 
    mandatory: false,
    basicProperties: null, 
    body: body
);

Console.WriteLine($"[x] Sent command : {message}");
Console.WriteLine("[x] Press enter to exit...");
Console.ReadLine();