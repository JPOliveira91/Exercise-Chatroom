using Chatroom.Bot.Class;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "stock_price", durable: false,
      exclusive: false, autoDelete: false, arguments: null);
    channel.BasicQos(0, 1, false);
    var consumer = new EventingBasicConsumer(channel);
    channel.BasicConsume(queue: "stock_price",
      autoAck: false, consumer: consumer);
    Console.WriteLine(" [x] Awaiting RPC requests");

    consumer.Received += (model, ea) =>
    {
        string? response = null;

        var body = ea.Body.ToArray();
        var props = ea.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        var stockCode = Encoding.UTF8.GetString(body);

        try
        {
            Console.WriteLine(" [.] RetrieveStockPrice({0})", stockCode);
            var apiIntegration = new APIIntegration();
            var processStockPrice = new ProcessStockPrice(apiIntegration);
            if (!String.IsNullOrEmpty(stockCode))
            {
                response = string.Format("{0} quote is ${1} per share", stockCode, processStockPrice.RetrieveStockPrice(stockCode));
            }
            else
            {
                response = "Empty stockcode send.";
                Console.WriteLine(" [.] " + response);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(" [.] " + e.Message);
            response = string.Format("Error while retrieving stock price for {0}.", stockCode);
        }
        finally
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);
            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
              basicProperties: replyProps, body: responseBytes);
            channel.BasicAck(deliveryTag: ea.DeliveryTag,
              multiple: false);
        }
    };

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}
