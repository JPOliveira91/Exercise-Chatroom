using Chatroom.Bot.Interface;
using System.Net.Http.Headers;

namespace Chatroom.Bot.Class
{
    public class APIIntegration : IAPIIntegration
    {
        public HttpResponseMessage CallAPI(string stockCode)
        {
            var url = "https://stooq.com/q/l/";
            var urlParameters = string.Format("?s={0}&f=sd2t2ohlcv&h&e=csv", stockCode.ToLower());

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            return response;
        }
    }
}
