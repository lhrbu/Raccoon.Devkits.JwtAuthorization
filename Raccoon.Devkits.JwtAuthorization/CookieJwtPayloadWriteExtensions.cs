using JWT;
using JWT.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
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

namespace Raccoon.Devkits.JwtAuthorization
{
    public static class CookieJwtPayloadWriteExtensions
    {
        public static void Write<TPayload>(this ControllerBase controller,string cookieName,TPayload payload)
        {
            controller.HttpContext.Response.Cookies.Append(cookieName,
            )
        }
    }
}