using System.Globalization;
using Chatroom.Bot.Interface;

namespace Chatroom.Bot.Class
{
    public class ProcessStockPrice : IProcessStockPrice
    {
        public readonly IAPIIntegration _apiIntegration;

        public ProcessStockPrice()
        {
            _apiIntegration = new APIIntegration();
        }

        public ProcessStockPrice(IAPIIntegration apiIntegration)
        {
            _apiIntegration = apiIntegration;
        }

        public string RetrieveStockPrice(string stockCode)
        {
            var csvStream = RetrieveCSVFromAPI(stockCode);
            var rawstockPrice = RetrieveRawStockPriceFromCSV(stockCode, csvStream);

            return ProcessRawStockPrice(stockCode, rawstockPrice);
        }

        public StreamReader RetrieveCSVFromAPI(string stockCode)
        {
            StreamReader csvStream = StreamReader.Null;

            HttpResponseMessage response = _apiIntegration.CallAPI(stockCode);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                csvStream = new StreamReader(response.Content.ReadAsStreamAsync().Result);  //Make sure to add a reference to System.Net.Http.Formatting.dll
            }
            else
            {
                throw new Exception(string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
            }

            if (DataIsEmpty(csvStream)) throw new Exception(String.Format("Empty data retrieved for stockcode {0}.", stockCode));

            return csvStream;
        }

        private static bool DataIsEmpty(StreamReader csvStream)
        {            
            if (csvStream == StreamReader.Null) return true;

            if (csvStream.Peek() == -1) return true;

            return false;
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
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while retrieving price in CSV for stock {0}.", stockCode), ex);
            }
        }

        public string ProcessRawStockPrice(string stockCode, string rawstockPrice)
        {
            decimal decStockPrice;
            var style = NumberStyles.Number;
            var culture = CultureInfo.CreateSpecificCulture("en-US");

            if (!Decimal.TryParse(rawstockPrice, style, culture, out decStockPrice)) throw new Exception(String.Format("Invalid price in retrieved CSV for stockcode {0}.", stockCode));

            return decStockPrice.ToString("0.00", culture);
        }
    }
}
