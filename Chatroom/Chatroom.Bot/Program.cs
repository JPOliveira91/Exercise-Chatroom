using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http.Headers;
using System.Text;
using System.Globalization;

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
        string response = null;

        var body = ea.Body.ToArray();
        var props = ea.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        var stockCode = Encoding.UTF8.GetString(body);

        try
        {
            Console.WriteLine(" [.] RetrieveStockPrice({0})", stockCode);
            response = string.Format("{0} quote is ${1} per share", stockCode, RetrieveStockPriceFromAPI(stockCode));
        }
        catch (Exception e)
        {
            Console.WriteLine(" [.] " + e.Message);
            response = string.Format("Error while retrieving stock price for {0}. Please try again!", stockCode);
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

string RetrieveStockPriceFromAPI(string stockCode)
{
    var url = "https://stooq.com/q/l/";
    var urlParameters = string.Format("?s={0}&f=sd2t2ohlcv&h&e=csv", stockCode.ToLower());
    decimal decStockPrice = 0;

    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri(url);

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    // List data response.
    HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
    if (response.IsSuccessStatusCode)
    {        
        // Parse the response body.
        var csvStream = response.Content.ReadAsStreamAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll

        var style = NumberStyles.Number;
        var stockPrice = RetrieveStockPriceFromCSV(csvStream);
        var culture = CultureInfo.CreateSpecificCulture("en-US");

        if (!Decimal.TryParse(stockPrice, style, culture, out decStockPrice)) throw new Exception("Invalid stock price.");
    }
    else
    {
        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
    }

    // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
    client.Dispose();

    return decStockPrice.ToString("0.00");
}

string RetrieveStockPriceFromCSV(Stream csvStream)
{
    StreamReader sr = new StreamReader(csvStream);

    string line;
    string[] column = new string[7];
    bool firstLine = true;
    
    // Headers Line
    line = sr.ReadLine();

    // Values Line
    line = sr.ReadLine();
    column = line.Split(',');

    return column[6];
}
