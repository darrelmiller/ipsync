using Microsoft.Extensions.Configuration;
using System.IO;

namespace SyncIp
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var syncIpService = new SyncIpService(config);
            syncIpService.SyncIP().Wait();
        }
    }
}
