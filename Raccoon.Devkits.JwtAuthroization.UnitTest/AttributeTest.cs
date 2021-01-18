using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Raccoon.Devkits.JwtAuthroization.Services;
using Raccoon.Devkits.JwtAuthroization.TestServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Raccoon.Devkits.JwtAuthroization.UnitTest
{
    public class AttributeTest
    {
        private readonly Microsoft.AspNetCore.TestHost.TestServer _server;
        public AttributeTest()
        {
            _server =new(WebHost.CreateDefaultBuilder().UseStartup<Startup>());
        }
        [Fact]
        public async Task CookieJwtPayloadRuleAttributeTest()
        {
            using HttpClient client = _server.CreateClient();
            JwtEncodeService jwtEncodeService = _server.Services.GetRequiredService<JwtEncodeService>();
            HttpRequestMessage httpRequest = new(HttpMethod.Get, "/WeatherForecast");
            IDictionary<string, object> payload = new Dictionary<string, object>
            {
                {"rh1","value1" }
            };
            httpRequest.Headers.Add("Cookie", $"testcookie={jwtEncodeService.Encode(payload,"secret")}");
            var res = await client.SendAsync(httpRequest);
            var values = res.Content.ReadFromJsonAsync<WeatherForecast[]>();
        }
    }
}
