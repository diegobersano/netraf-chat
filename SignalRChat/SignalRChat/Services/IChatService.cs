using SignalRChat.Models;
using System;
using System.Threading.Tasks;

namespace SignalRChat.Services
{
    public interface IChatServices
    {
        Task Connect(string userName);

        Task Send(string name, string message);

        event EventHandler<Message> OnMessageReceived;
        event EventHandler<string> OnAudioRecognized;
    }
}