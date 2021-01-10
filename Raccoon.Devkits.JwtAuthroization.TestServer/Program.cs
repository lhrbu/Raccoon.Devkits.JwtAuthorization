using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raccoon.Devkits.JwtAuthroization.Models;
using Raccoon.Devkits.JwtAuthroization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.TestServer
{
   
    public class Program
    {
        public static void TestJwtToken()
        {
            var payload = new Dictionary<string, object> {
                { "claim1","value1"},
                { "claim2",22},
                { "exp",DateTimeOffset.Now.AddSeconds(12000).ToUnixTimeSeconds()}
                };
            JwtEncodeService encodeService = new();
            var token = encodeService.Encode(payload, "secret");
            var payload2 = encodeService.Decode(token, "secret");
            var reToken = encodeService.RefreshToken(token, "secret", 30000);
            var repayload2 = encodeService.Decode(reToken, "secret");
        }
        public static void Main(string[] args)
        {
            TestJwtToken();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
