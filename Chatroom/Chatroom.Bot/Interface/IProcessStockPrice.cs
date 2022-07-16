
namespace Chatroom.Bot.Interface
{
    public interface IProcessStockPrice
    {
        string ProcessRawStockPrice(string stockCode, string rawstockPrice);
        StreamReader RetrieveCSVFromAPI(string stockCode);
        string RetrieveRawStockPriceFromCSV(string stockCode, StreamReader csvStream);
        string RetrieveStockPrice(string stockCode);
    }
}