namespace SignalRChat
{
    public static class Constants
    {
        public static readonly string SpeechRecognitionEndpoint = "http://signalr-cognitive-chat.azurewebsites.net/api/SpeechRecognition";
        public static readonly string AudioContentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";

        public static readonly string AudioFilename = "Todo.wav";
    }
}