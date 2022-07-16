
namespace Chatroom.Bot.Interface
{
    public interface IAPIIntegration
    {
        HttpResponseMessage CallAPI(string stockCode);
    }
}