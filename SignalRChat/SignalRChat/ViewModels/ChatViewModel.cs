using SignalRChat.Models;
using System.Collections.ObjectModel;

namespace SignalRChat.ViewModels
{
    public class ChatViewModel
    {
        public ChatViewModel()
        {
            this.Messages = new ObservableCollection<Message>();
        }

        public string Text { get; set; }

        public ObservableCollection<Message> Messages { get; set; }
    }
}