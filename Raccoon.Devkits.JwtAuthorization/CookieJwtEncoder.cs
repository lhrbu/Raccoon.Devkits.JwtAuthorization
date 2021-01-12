using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Raccoon.Devkits.JwtAuthroization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthorization
{
    public class CookieJwtEncoder
    {
        private readonly JwtEncodeService _jwtEncodeService;
        private readonly IConfiguration _configuration;
        public CookieJwtEncoder(
            JwtEncodeService jwtEncodeService,
            IConfiguration configuration)
        {
            _jwtEncodeService = jwtEncodeService;
            _configuration = configuration;
        }

        public void EncodeHttpContext(HttpContext httpContext,string cookieName,IDictionary<string,object> payload)
        {
            string secret = _configuration["JwtSecret"];
            httpContext.Response.Cookies.Append(cookieName,
                _jwtEncodeService.Encode(payload, secret));
        }
    }
}
