using System;

namespace SignalRChat.Contracts
{
    public class Message
    {
        public string UserName { get; set; }

        public string Text { get; set; }

        public DateTime SendDate { get; set; }

        public string SendDetail => $"{UserName} | {SendDate:G}";
    }
}