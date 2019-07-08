using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SyncIp
{
    public partial class SyncIpService
    {
        public class JsonContent : HttpContent
        {
            private readonly JObject content;

            public JsonContent(JObject content)
            {
                this.content = content;
                this.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                var sw = new StreamWriter(stream);
                await content.WriteToAsync(new JsonTextWriter(sw));
                await sw.FlushAsync();
            }

            protected override bool TryComputeLength(out long length)
            {
                length = -1;
                return false;
            }
        }
    }
}
