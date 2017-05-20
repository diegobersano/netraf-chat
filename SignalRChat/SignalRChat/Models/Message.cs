using System;

namespace SignalRChat.Models
{
    public class Message
    {
        public string UserName { get; set; }

        public string Text { get; set; }

        public DateTime SendDate { get; set; }

        public string SendDetail => $"{this.UserName} | {this.SendDate:G}";
    }
}