using JWT;
using JWT.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raccoon.Devkits.JwtAuthroization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.Models
{
    public class CookieJwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        public CookieJwtAuthorizationMiddleware(
            RequestDelegate next)
        { 
            _next = next;
        }
       
        public async Task InvokeAsync(HttpContext context,IConfiguration configuration)
        { 
            IEnumerable<CookieJwtPayloadRequirementAttribute>? payloadAttributes = context.Features.Get<IEndpointFeature>()?
                .Endpoint?.Metadata?.Where(item => item is CookieJwtPayloadRequirementAttribute).Select(item => (item as CookieJwtPayloadRequirementAttribute)!);
            if(payloadAttributes == null || payloadAttributes?.Count()==0) 
            { 
                await _next(context); 
                return;
            }

            string secret = configuration["JwtSecret"];
            JwtEncodeService jwtEncodeService = (context.RequestServices.GetService(typeof(JwtEncodeService)) as JwtEncodeService)!;
            foreach (var requirementAttribute in payloadAttributes!)
            {
                string key = requirementAttribute.Key;
                object[] requirments = requirementAttribute.Values;
                KeyValuePair<string, string> cookie = context.Request.Cookies
                   .FirstOrDefault(item => item.Key == requirementAttribute.CookieName);

                try
                {
                    IDictionary<string, object> cookiePayload = jwtEncodeService.Decode(cookie.Value, secret);
                    if (!cookiePayload.ContainsKey(key) || !requirments.Contains(cookiePayload[key]))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }
                catch { 
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }
            await _next(context);
            
        }
    }
}
