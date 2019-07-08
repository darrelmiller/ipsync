using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SyncIp
{
    public class ArmAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IConfidentialClientApplication app;

        public ArmAuthenticationProvider(IConfidentialClientApplication app)
        {
            this.app = app;
        }
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var result = await app.AcquireTokenForClient(new string[] { "https://management.azure.com/.default" })
                                .ExecuteAsync();

            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);
        }
    }
}
