using SignalRChat.Web.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace SignalRChat.Web.Controllers
{
    public class SpeechRecognitionController : ApiController
    {
        // POST api/<controller>
        public async Task<IHttpActionResult> Post(string connectionId)
        {
            var service = new SpeechRecognitionService();

            var content = await Request.Content.ReadAsByteArrayAsync();

            service.Recognice(connectionId, content);

            return Ok();
        }
    }
}