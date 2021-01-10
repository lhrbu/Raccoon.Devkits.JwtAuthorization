using JWT;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Raccoon.Devkits.JwtAuthroization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.Models
{
    public static class JwtAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtAuthorization(this IApplicationBuilder app)=>
        
            app.UseMiddleware<JwtAuthorizationMiddleware>();
        
    }
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtEncodeService _jwtEncodeService;
        private readonly string _secret=string.Empty;
        public JwtAuthorizationMiddleware(
            RequestDelegate next,
            JwtEncodeService jwtEncodeService)
        { 
            _next = next;
            _jwtEncodeService = jwtEncodeService;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            JwtPayloadAttribute? payloadAttribute = context.Features.Get<IEndpointFeature>()
                .Endpoint.Metadata.GetMetadata<JwtPayloadAttribute>();
            if (payloadAttribute is not null) 
            {
                KeyValuePair<string,string>? cookie = context.Request.Cookies
                    .FirstOrDefault(item => item.Key == payloadAttribute.CookieName);
                if (cookie is null) { throw new InvalidOperationException(); }
                var payload = _jwtEncodeService.Decode(cookie.Value.Value, _secret);
                var requirements = payloadAttribute.Requirements;
                foreach(var requirement in requirements)
                {
                    string key = requirement.Key;
                    if (!payload.ContainsKey(key) || payload[key]!=requirement.Value)
                    {
                        throw new Exception();
                    }
                }
                
            }
            await _next(context);
        }
    }
}
