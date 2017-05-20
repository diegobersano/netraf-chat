using Microsoft.AspNet.SignalR.Client;
using Plugin.Toasts;
using SignalRChat.Models;
using SignalRChat.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ChatServices))]
namespace SignalRChat.Services
{
    public class ChatServices : IChatServices
    {
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;

        public event EventHandler<Message> OnMessageReceived;
        public event EventHandler<string> OnAudioRecognized;

        public ChatServices()
        {
            _connection = new HubConnection("http://signalr-cognitive-chat.azurewebsites.net/");
            _proxy = _connection.CreateHubProxy("ChatHub");
        }

        #region IChatServices implementation

        public async Task Connect(string userName)
        {
            await _connection.Start();

            App.ConnectionId = _connection.ConnectionId;

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

            _proxy.On("NewUser", async (string name) =>
            {
                var notificator = DependencyService.Get<IToastNotificator>();

                var options = new NotificationOptions
                {
                    Title = $"{name} está conectado al chat",
                    Description = $"{name} está conectado al chat"
                };

                await notificator.Notify(options);
            });
        }

        public async Task Send(string name, string message)
        {
            await _proxy.Invoke("Send", name, message);
        }

        #endregion
    }
}