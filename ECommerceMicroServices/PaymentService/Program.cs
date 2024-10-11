using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost"};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

var exchangeName = "order";
var paymentQueueName = "payment_queue";
var routingKey = "order.new";

channel.ExchangeDeclare(
    exchange: exchangeName, 
    type: ExchangeType.Direct
);

channel.QueueDeclare(
    queue: paymentQueueName,
    durable: true, 
    exclusive: false,
    autoDelete: false,
    arguments: null
);

channel.QueueBind(
    queue: paymentQueueName, 
    exchange: exchangeName,
    routingKey: routingKey
);

var paymentExchangeName = "payment";

channel.ExchangeDeclare(
    exchange: paymentExchangeName, 
    type: ExchangeType.Fanout
);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) => 
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] Order received : {message}");
    Console.WriteLine($"[x] Payment ongoing...");
    Thread.Sleep(2000);
    Console.WriteLine($"[x] Payment succeed !");

    var paymentMessages = $"Payment complete for order : {message}";
    var paymentBody = Encoding.UTF8.GetBytes(paymentMessages);

    channel.BasicPublish(
        exchange: paymentExchangeName, 
        routingKey: "", 
        basicProperties: null, 
        body: paymentBody
    );

    Console.WriteLine($"[x] Confirmation sent for order : {message}");
};

channel.BasicConsume(
    queue: paymentQueueName, 
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("[x] Press enter to exit...");
Console.ReadLine();