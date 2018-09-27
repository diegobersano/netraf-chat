using System;
using System.Threading.Tasks;

namespace SignalRChat.Contracts
{
    public interface IChatService
    {
        Task Connect(string userName);

        Task Send(string name, string message);

        event EventHandler<Message> OnMessageReceived;
        event EventHandler<string> OnAudioRecognized;
        event EventHandler<string> OnNewUser;
    }
}