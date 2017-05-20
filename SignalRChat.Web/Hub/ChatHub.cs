namespace SignalRChat.Web.Hub
{
    public class ChatHub : Microsoft.AspNet.SignalR.Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }

        public void Login(string userName)
        {
            Clients.Others.newUser(userName);
        }
    }
}