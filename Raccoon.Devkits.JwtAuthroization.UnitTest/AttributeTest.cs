using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raccoon.Devkits.JwtAuthroization.Services;
using Raccoon.Devkits.JwtAuthroization.TestServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private void WriteCookie(HttpRequestMessage httpRequest,IDictionary<string,object> payload)
        {
            JwtEncodeService jwtEncodeService = _server.Services.GetRequiredService<JwtEncodeService>();
            IConfiguration configuration = _server.Services.GetRequiredService<IConfiguration>();
            string encodedPayload = jwtEncodeService.Encode(payload, configuration["JwtSecret"]);
            httpRequest.Headers.Add("Cookie", $"testcookie={encodedPayload}");
        }
        [Fact]
        public async Task TestConfigAsync()
        {
            using HttpClient client = _server.CreateClient();
            HttpRequestMessage httpRequest = new(HttpMethod.Get, "/WeatherForecast");
            IDictionary<string, object> payload = new Dictionary<string, object>
            {
                {"testconfig","value8" }
            };
            WriteCookie(httpRequest, payload);
            var res = await client.SendAsync(httpRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
            //var values = res.Content.ReadFromJsonAsync<WeatherForecast[]>();
        }

        [Fact]
        public async Task TestMethodAsync()
        {
            using HttpClient client = _server.CreateClient();
            HttpRequestMessage httpRequest = new(HttpMethod.Get, "/WeatherForecast/Method");
            IDictionary<string, object> payload = new Dictionary<string, object>
            {
                {"testmethod","value1" }
            };
            WriteCookie(httpRequest, payload);
            var res = await client.SendAsync(httpRequest);
            Assert.NotEmpty(await res.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>());
        }

        [Fact]
        public async Task TestControllerAsync()
        {
            using HttpClient client = _server.CreateClient();
            HttpRequestMessage httpRequest = new(HttpMethod.Get, "/WeatherForcastAuthorization");
            IDictionary<string, object> payload = new Dictionary<string, object>
            {
                {"testcontroller","value3" }
            };
            WriteCookie(httpRequest, payload);
            var res = await client.SendAsync(httpRequest);
            Assert.NotEqual(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task TestWithoutCookieAsync()
        {
            using HttpClient client = _server.CreateClient();
            HttpRequestMessage httpRequest = new(HttpMethod.Get, "/WeatherForecast");
            var res = await client.SendAsync(httpRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task TestPathNotFount()
        {
            using HttpClient client = _server.CreateClient();
            HttpRequestMessage httpRequest = new(HttpMethod.Get, "/WeatherForcastAuthorization/Hahaha");
            IDictionary<string, object> payload = new Dictionary<string, object>
            {
                {"testcontroller","value3" }
            };
            WriteCookie(httpRequest, payload);
            var res = await client.SendAsync(httpRequest);
            Assert.NotEqual(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task TestTrivialAsync()
        {
            using HttpClient client = _server.CreateClient();
            HttpRequestMessage httpRequest = new(HttpMethod.Get, "/Trivial");
            var res = await client.SendAsync(httpRequest);
            Assert.Equal("Hello World", await res.Content.ReadAsStringAsync());
        }
    }
}
