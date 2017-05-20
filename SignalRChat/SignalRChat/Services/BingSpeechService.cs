using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SignalRChat.Services
{
    public class BingSpeechService
    {
        public async Task RecognizeSpeechAsync(string filename)
        {
            // Read audio file to a stream
            var file = await PCLStorage.FileSystem.Current.LocalStorage.GetFileAsync(filename);
            var fileStream = await file.OpenAsync(PCLStorage.FileAccess.Read);

            // Send audio stream to Bing and deserialize the response
            string requestUri = GenerateRequestUri(Constants.SpeechRecognitionEndpoint);
            var response = await SendRequestAsync(fileStream, requestUri, Constants.AudioContentType);

            fileStream.Dispose();
        }

        private static string GenerateRequestUri(string speechEndpoint)
        {
            var requestUri = speechEndpoint;
            requestUri += $"?connectionId={App.ConnectionId}";

            return requestUri;
        }

        private static async Task<HttpStatusCode> SendRequestAsync(Stream fileStream, string url, string contentType)
        {
            var content = new StreamContent(fileStream);
            content.Headers.TryAddWithoutValidation("Content-Type", contentType);

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(url, content);

                return response.StatusCode;
            }
        }
    }
}