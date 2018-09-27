using Microsoft.AspNet.SignalR.Client;
using SignalRChat.Contracts;
using SignalRChat.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ChatService))]
namespace SignalRChat.Services
{
    public class ChatService : IChatService
    {
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;

        internal static string ConnectionId;

        public event EventHandler<Message> OnMessageReceived;
        public event EventHandler<string> OnAudioRecognized;
        public event EventHandler<string> OnNewUser;

        public ChatService()
        {
            _connection = new HubConnection("http://signalr-cognitive-chat.azurewebsites.net/");
            _proxy = _connection.CreateHubProxy("ChatHub");
        }

        #region IChatServices implementation

        public async Task Connect(string userName)
        {
            await _connection.Start();

            ConnectionId = _connection.ConnectionId;

            await _proxy.Invoke("Login", userName);

            _proxy.On("AddNewMessageToPage", (string name, string message) =>
            {
                OnMessageReceived?.Invoke(this, new Message
                {
                    UserName = name,
                    Text = message,
                    SendDate = DateTime.Now
                });
            });

            _proxy.On("AudioRecognized", (string text) =>
            {
                OnAudioRecognized?.Invoke(this, text);
            });

            _proxy.On("NewUser", (string name) =>
            {
                OnNewUser?.Invoke(this, name);
            });
        }

        public async Task Send(string name, string message)
        {
            await _proxy.Invoke("Send", name, message);
        }

        #endregion
    }
}