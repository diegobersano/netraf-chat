using SignalRChat.Contracts;
using SignalRChat.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(BingSpeechService))]
namespace SignalRChat.Services
{
    public class BingSpeechService : ISpeechService
    {
        public async Task RecognizeSpeechAsync(string fileName)
        {
            // Read audio file to a stream
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var file = File.ReadAllBytes(Path.Combine(localFolder, fileName));
            Stream fileStream = new MemoryStream(file);

            // Send audio stream to Bing and deserialize the response
            string requestUri = GenerateRequestUri(Constants.SpeechRecognitionEndpoint);
            var response = await SendRequestAsync(fileStream, requestUri, Constants.AudioContentType);

            fileStream.Dispose();
        }

        private static string GenerateRequestUri(string speechEndpoint)
        {
            var requestUri = speechEndpoint;
            requestUri += $"?connectionId={ChatService.ConnectionId}";

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