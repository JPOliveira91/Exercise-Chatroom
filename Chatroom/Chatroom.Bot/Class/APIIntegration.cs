using Chatroom.Bot.Interface;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace Chatroom.Bot.Class
{
    public class APIIntegration : IAPIIntegration
    {
        public HttpResponseMessage CallAPI(string stockCode)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");

            var config = configuration.Build();

            var url = config["StockPriceAPI:URL"];
            var urlParameters = string.Format(config["StockPriceAPI:Parameter"], stockCode.ToLower());

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            return response;
        }
    }
}
