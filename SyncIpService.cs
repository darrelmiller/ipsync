using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyncIp
{
    public partial class SyncIpService
    {
        private IConfidentialClientApplication app;
        private HttpClient armClient;
        private HttpClient simpleClient;
        private string subscriptionId;
        private string zone;
        private string host;

        public SyncIpService(IConfigurationRoot config)
        {
            zone = config["zone"];
            host = config["host"];
            subscriptionId = config["subscriptionId"];
            app = ConfidentialClientApplicationBuilder.Create(config["clientId"])
               .WithTenantId(config["tenantId"])
               .WithClientSecret(config["clientSecret"])
               .Build();
            var auth = new ArmAuthenticationProvider(app);

            armClient = GraphClientFactory.Create(auth);
            simpleClient = new HttpClient();
         }

        public async Task SyncIP()
        {
            // Get IP
            var ipPayload = await simpleClient.GetStringAsync("https://helloacm.com/api/what-is-my-ip-address/");
            var ipToken = JToken.Parse(ipPayload);
            var ip = ipToken.Value<string>();
            Console.WriteLine("Current public IP: " + ip);

            // Get DNS record
            var dnsPayloadResponse = await armClient.SendAsync(GetDNSRecordRequest(subscriptionId, zone, host));
            dnsPayloadResponse.EnsureSuccessStatusCode();
            var dnsObject = await GetJObjectResponseAsync(dnsPayloadResponse);
            var currentDNSIp = (string)(dnsObject["properties"]["ARecords"][0]["ipv4Address"]);
            Console.WriteLine($"Current IP for {host}.{zone} is {currentDNSIp}");


            if (ip != currentDNSIp)
            {
                // Patch DNS record
                Console.WriteLine($"Updating IP to {ip}");
                var dnsUpdateResponse = await armClient.SendAsync(PatchDNSRecordRequest(subscriptionId, "tavis.net", "ash", ip));
                dnsUpdateResponse.EnsureSuccessStatusCode();
                Console.WriteLine($"Success!");
            } else
            {
                Console.WriteLine($"No update necessary");
            }
        }

        public HttpRequestMessage GetDNSRecordRequest(string subscriptionId, string zone, string host)
        {
            return new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/domains/providers/Microsoft.Network/dnsZones/{zone}/A/{host}?api-version=2018-05-01"),
                Method = HttpMethod.Get
            };
        }

        public HttpRequestMessage PatchDNSRecordRequest(string subscriptionId, string zone, string host, string newIp)
        {
            return new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/domains/providers/Microsoft.Network/dnsZones/{zone}/A/{host}?api-version=2018-05-01"),
                Method = new HttpMethod("PATCH"),
                Content = new JsonContent(new JObject(
                                            new JProperty("properties",
                                                new JObject(
                                                    new JProperty("ARecords",
                                                          new JArray(
                                                              new JObject(new JProperty("ipv4Address",new JValue(newIp)))
                                                              )
                                                        )
                                                    )
                                                )
                                            )
                                        )
            };
        }

        public async Task<JObject> GetJObjectResponseAsync(HttpResponseMessage response)
        {
            var payloadStream = await response.Content.ReadAsStreamAsync();
            return JObject.Load(new JsonTextReader(new StreamReader(payloadStream)));
        }

    }
}
