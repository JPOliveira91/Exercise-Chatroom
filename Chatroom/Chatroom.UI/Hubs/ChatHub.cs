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

        public async Task SendMessage(string room, string user, string message)
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
                UserName = user,
                Room = room
            });

            await Clients.All.SendAsync("ReceiveMessage", room, user, date.ToString(), message);
        }

        public string RetrieveStockPriceMessage(string stockCode)
        {
            string response;

            try
            {
                var rpcClient = new RpcClient();

                response = rpcClient.RetrieveStockPriceMessage(stockCode);

                rpcClient.Close();
            }
            catch (Exception ex)
            {
                response = String.Format("Error while retrieving stock price for {0}.", stockCode);
            }

            return response;
        }

        public void SaveMessage(Message message)
        {
            _context.Message.Add(message);
            var result = _context.SaveChanges();
        }
    }
}
