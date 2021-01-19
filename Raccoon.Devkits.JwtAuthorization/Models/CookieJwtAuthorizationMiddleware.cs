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
using System.Text.RegularExpressions;
using Serilog;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Raccoon.Devkits.JwtAuthroization.Models
{
    public class CookieJwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        
        public CookieJwtAuthorizationMiddleware(
            RequestDelegate next)
        { _next = next;}
        
        private IEnumerable<AuthorizationRule> GetAuthorizationRulesFromController(ControllerActionDescriptor controllerActionDescriptor)=>
            controllerActionDescriptor.ControllerTypeInfo
                .GetCustomAttributes<CookieJwtPayloadRuleAttribute>()
                .Select(item=>item.AuthorizationRule);
        private IEnumerable<AuthorizationRule> GetAuthorizationRulesFromMethod(ControllerActionDescriptor controllerActionDescriptor) =>
            controllerActionDescriptor.MethodInfo
                .GetCustomAttributes<CookieJwtPayloadRuleAttribute>()
                .Select(item => item.AuthorizationRule);

        //private bool WildCardCompare(string value,string pattern)
        //{
        //    string regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        //    return Regex.IsMatch(value,regexPattern);
        //}

        private bool IsSubUrl(string value,string parentUrl)
        {
            if(parentUrl == "/") { return true; }
            PathString urlPathString = new(parentUrl);
            PathString valuePathString = new(value);
            return urlPathString.StartsWithSegments(valuePathString);
        }
        
        private bool AuthorizeRules(HttpContext context, IEnumerable<AuthorizationRule> rules)
        {
            JwtEncodeService jwtEncodeService = context.RequestServices.GetRequiredService<JwtEncodeService>();
            IConfiguration configuration = context.RequestServices.GetRequiredService<IConfiguration>();

            foreach (AuthorizationRule rule in rules)
            {
                KeyValuePair<string, string> cookie = context.Request.Cookies
                   .FirstOrDefault(item => item.Key == rule.CookieName);
                if (cookie.Equals(default(KeyValuePair<string, string>))) { return false; }
                IDictionary<string, object> cookiePayload = jwtEncodeService.Decode(cookie.Value, configuration["JwtSecret"]);
                if (!cookiePayload.ContainsKey(rule.RequiredHeader) ||
                    !rule.AllowedRange.Contains(cookiePayload[rule.RequiredHeader]))
                { return false; }
            }
            return true;
        }


        public async Task InvokeAsync(HttpContext context,IOptionsMonitor<CookieJwtOptions> options)
        {
            if(!options.CurrentValue.Enable.HasValue || !options.CurrentValue.Enable.Value)
            {
                await _next(context);
                return;
            }
            // In case of Path not exists!
            ControllerActionDescriptor? controllerActionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>()!;
            if(controllerActionDescriptor is null) { await _next(context);return; }

            try
            {
                //ControllerActionDescriptor controllerActionDescriptor = (context.Features
                //    .Get<IEndpointFeature>()?.Endpoint?.Metadata?
                //    .FirstOrDefault(item => item is ControllerActionDescriptor) as ControllerActionDescriptor)!;

                IEnumerable<AuthorizationRule> authorizationRulesFromMethod = GetAuthorizationRulesFromMethod(controllerActionDescriptor);
                if(authorizationRulesFromMethod.Count()>0)
                {
                    if(AuthorizeRules(context,authorizationRulesFromMethod))
                    { await _next(context);}
                    else
                    { context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; }
                    return;
                }

                IEnumerable<AuthorizationRule> authorizationRulesFromController = GetAuthorizationRulesFromController(controllerActionDescriptor);
                if(authorizationRulesFromController.Count()>0)
                {
                    if(AuthorizeRules(context,authorizationRulesFromController))
                    { await _next(context);}
                    else { context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; }
                    return;
                }

                IEnumerable<AuthorizationRule> authorizationRulesFromConfig = options.CurrentValue.Rules?
                    .Where(item => IsSubUrl(context.Request.Path, item.PathPattern)) ?? 
                    Enumerable.Empty<AuthorizationRule>();
                
                if(authorizationRulesFromConfig.Count()>0)
                {
                    if(AuthorizeRules(context,authorizationRulesFromConfig))
                    { await _next(context);}
                    else { context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; }
                    return;
                }

                await _next(context);
            }catch(Exception exception){
                Log.Logger.Error("{exception}", exception);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
            
        }
    }
}
