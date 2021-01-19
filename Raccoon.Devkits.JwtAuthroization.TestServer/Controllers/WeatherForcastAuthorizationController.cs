using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raccoon.Devkits.JwtAuthroization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.TestServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [CookieJwtPayloadRule("testcookie", "testcontroller", "value3", "value4")]
    public class WeatherForcastAuthorizationController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
