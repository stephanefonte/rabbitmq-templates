using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

var exchangeName = "payment";

channel.ExchangeDeclare(
    exchange: exchangeName,
    type: ExchangeType.Fanout
);

var queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(
    queue: queueName,
    exchange: exchangeName,
    routingKey: ""
);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) => {
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"[x] Archive order : {message}");
};

channel.BasicConsume(
    queue: queueName,
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("[x] Press enter to exit...");
Console.ReadLine();