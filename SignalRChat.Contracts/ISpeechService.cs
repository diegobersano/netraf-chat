using System.Threading.Tasks;

namespace SignalRChat.Contracts
{
    public interface ISpeechService
    {
        Task RecognizeSpeechAsync(string fileName);
    }
}