using Chatroom.UI.Data;
using Chatroom.UI.Models;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatroomUIContext _context;

        public ChatHub(
            ChatroomUIContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            if (message.StartsWith("/stock="))
            {
                var stockCode = message.Replace("/stock=", "").ToUpper();

                message = RetrieveStockPriceMessage(stockCode);
                user = "STOCK_BOT";
            }

            var date = DateTime.Now;

            SaveMessage(new Message() {
                Date = date,
                Text = message,
                UserName = user
            });

            await Clients.All.SendAsync("ReceiveMessage", user, date.ToString(), message);
        }

        public string RetrieveStockPriceMessage(string stockCode)
        {
            var rpcClient = new RpcClient();

            var response = rpcClient.RetrieveStockPrice(stockCode);

            rpcClient.Close();

            return response;
        }

        public void SaveMessage(Message message)
        {
            _context.Message.Add(message);
            var result = _context.SaveChanges();
        }
    }
}
