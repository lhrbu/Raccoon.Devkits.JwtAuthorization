using JWT;
using JWT.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Raccoon.Devkits.JwtAuthorization.Models;
using Raccoon.Devkits.JwtAuthroization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;

namespace Raccoon.Devkits.JwtAuthorization
{
    public static class CookieJwtPayloadWriteExtensions
    {
        public static void Write<TPayload>(this HttpContext context,string cookieName,TPayload payload)
        {
            IConfiguration configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            JwtEncodeService jwtEncodeService = context.RequestServices.GetRequiredService<JwtEncodeService>();
            string payloadString = JsonSerializer.Serialize(payload);
            IDictionary<string, object> payloadDict = (JsonSerializer.Deserialize<IDictionary<string, object>>(payloadString))!;
            context.Response.Cookies.Append(cookieName, jwtEncodeService.Encode(payloadDict, configuration["JwtSecret"]));
        }

        public static void Write(this HttpContext context, string cookieName,IDictionary<string,object> payload)
        {
            IConfiguration configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            JwtEncodeService jwtEncodeService = context.RequestServices.GetRequiredService<JwtEncodeService>();
            context.Response.Cookies.Append(cookieName, jwtEncodeService.Encode(payload, configuration["JwtSecret"]));
        }
    }
}