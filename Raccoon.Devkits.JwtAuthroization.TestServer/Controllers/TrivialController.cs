using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Raccoon.Devkits.JwtAuthorization;

namespace Raccoon.Devkits.JwtAuthroization.TestServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TrivialController : ControllerBase
    {
        [Consumes(MediaTypeNames.Text.Plain)]
        public string Get() => "Hello World";

        public void TestWriteTypePayload()
        {

        }
    }

    public record TestPayload(string Name,int Value);
}
