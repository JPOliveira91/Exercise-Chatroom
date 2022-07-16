using System.Net.Http.Headers;
using System.Globalization;

namespace Chatroom.Bot
{
    public class ProcessStockPrice
    {
        public string RetrieveStockPrice(string stockCode)
        {
            var csvStream = RetrieveCSVFromAPI(stockCode);
            var rawstockPrice = RetrieveRawStockPriceFromCSV(stockCode, csvStream);

            return ProcessRawStockPrice(stockCode, rawstockPrice);
        }

        public string ProcessRawStockPrice(string stockCode, string rawstockPrice)
        {
            decimal decStockPrice;
            var style = NumberStyles.Number;
            var culture = CultureInfo.CreateSpecificCulture("en-US");

            if (!Decimal.TryParse(rawstockPrice, style, culture, out decStockPrice)) throw new Exception(String.Format("Invalid price in retrieved CSV for stockcode {0}.", stockCode));

            return decStockPrice.ToString("0.00", culture);
        }

        public StreamReader RetrieveCSVFromAPI(string stockCode)
        {
            var url = "https://stooq.com/q/l/";
            var urlParameters = string.Format("?s={0}&f=sd2t2ohlcv&h&e=csv", stockCode.ToLower());

            HttpClient client = new HttpClient();
            StreamReader csvStream = StreamReader.Null;
            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                csvStream = new StreamReader(response.Content.ReadAsStreamAsync().Result);  //Make sure to add a reference to System.Net.Http.Formatting.dll
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();

            if (csvStream == StreamReader.Null) throw new Exception(String.Format("CSV was not retrieved for stockcode {0}.", stockCode));

            return csvStream;
        }

        public string RetrieveRawStockPriceFromCSV(string stockCode, StreamReader csvStream)
        {
            try
            {
                string line;
                string[] column = new string[7];

                // Headers Line
                csvStream.ReadLine();

                // Values Line
                line = csvStream.ReadLine();
                column = line.Split(',');

                return column[6];
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Error while retrieving price in CSV for stock {0}.", stockCode), ex);
            }
        }
    }
}
