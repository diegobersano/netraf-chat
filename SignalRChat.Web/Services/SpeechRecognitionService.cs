using Microsoft.AspNet.SignalR;
using Microsoft.CognitiveServices.SpeechRecognition;
using SignalRChat.Web.Hub;
using System.Linq;

namespace SignalRChat.Web.Services
{
    internal class SpeechRecognitionService
    {
        private DataRecognitionClient _dataClient;
        private string _connectionId;


        internal void Recognice(string connectionId, byte[] stream)
        {
            _connectionId = connectionId;

            _dataClient = SpeechRecognitionServiceFactory.CreateDataClient(
                SpeechRecognitionMode.LongDictation,
                "es-ES",
                "ebccef423cec4f098c35a774091582d4");

            _dataClient.OnResponseReceived += this.OnDataDictationResponseReceivedHandler;

            // Send of audio data to service. 
            this._dataClient.SendAudio(stream, stream.Length);
            this._dataClient.EndAudio();
        }

        private void OnDataDictationResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            if (e?.PhraseResponse?.Results != null && e.PhraseResponse.Results.Any())
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                context.Clients.Client(_connectionId).audioRecognized(e.PhraseResponse.Results[0]?.DisplayText);
            }
        }
    }
}