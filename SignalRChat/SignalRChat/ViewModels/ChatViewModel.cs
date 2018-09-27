using SignalRChat.Contracts;
using System.Collections.ObjectModel;

namespace SignalRChat.ViewModels
{
    public class ChatViewModel
    {
        public ChatViewModel()
        {
            Messages = new ObservableCollection<Message>();
        }

        public string Text { get; set; }

        public ObservableCollection<Message> Messages { get; set; }
    }
}