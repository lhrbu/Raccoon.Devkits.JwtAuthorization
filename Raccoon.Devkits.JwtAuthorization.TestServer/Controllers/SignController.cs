using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raccoon.Devkits.JwtAuthroization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.TestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignController : ControllerBase
    {
        private readonly JwtEncodeService _encodeService;
        public SignController(JwtEncodeService encodeService)
        { _encodeService = encodeService; }
        [HttpGet]
        public void Get()
        {
            string token = _encodeService.Encode(
                new Dictionary<string, object>
                {
                    {"key1","value1" },
                    {"exp",DateTimeOffset.Now.ToUnixTimeSeconds()+1200 }
                },"secret");
            HttpContext.Response.Cookies.Append("test", token);
        }
    }
}
