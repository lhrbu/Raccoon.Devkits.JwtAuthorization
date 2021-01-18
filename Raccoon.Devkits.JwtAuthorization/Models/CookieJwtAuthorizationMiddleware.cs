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
        
        private IEnumerable<AuthorizationRule> GetAuthorizationRulesFromController(HttpContext context)
        {
            IEnumerable<CookieJwtPayloadRuleAttribute>? payloadAttributes = context.Features.Get<IEndpointFeature>()?
                .Endpoint?.Metadata?.Where(item => item is CookieJwtPayloadRuleAttribute).Select(item => (item as CookieJwtPayloadRuleAttribute)!);
            if(payloadAttributes is null) { return Enumerable.Empty<AuthorizationRule>();}
            return payloadAttributes.Select(item=>item.AuthorizationRule with{PathPattern = context.Request.Path});
        }

        private bool WildCardCompare(string value,string pattern)
        {
            string regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";
            return Regex.IsMatch(value,regexPattern);
        }
        private bool Authorize(HttpContext context,AuthorizationRule rule)
        {
            JwtEncodeService jwtEncodeService = (context.RequestServices.GetService(typeof(JwtEncodeService)) as JwtEncodeService)!;
            KeyValuePair<string, string> cookie = context.Request.Cookies
                   .FirstOrDefault(item => item.Key == rule.CookieName);
            IDictionary<string, object> cookiePayload = jwtEncodeService.Decode(cookie.Value, "secret");
            return cookiePayload.ContainsKey(rule.RequiredHeader) && rule.AllowedRange.Contains(cookiePayload[rule.RequiredHeader]);
        }
        public async Task InvokeAsync(HttpContext context,IOptionsMonitor<CookieJwtOptions> options)
        {
            if(!options.CurrentValue.Enable.HasValue || !options.CurrentValue.Enable.Value)
            {
                await _next(context);
                return;
            }

            try{
                IEnumerable<AuthorizationRule> authorizationRulesFromConfig = options.CurrentValue.Rules?
                    .Where(item=>WildCardCompare(context.Request.Path,item.PathPattern))??
                    Enumerable.Empty<AuthorizationRule>();
                IEnumerable<AuthorizationRule> authorizationRulesFromController = GetAuthorizationRulesFromController(context);
            
                bool authorizedFlag = true;
                foreach (AuthorizationRule rule in authorizationRulesFromConfig)
                {
                    authorizedFlag = Authorize(context, rule);
                    if (!authorizedFlag) { break; }
                }
                foreach (AuthorizationRule rule in authorizationRulesFromController)
                {
                    authorizedFlag = Authorize(context, rule);
                    if (!authorizedFlag) { break; }
                }

                if (authorizedFlag) { await _next(context); }
            }catch{
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
            
        }
    }
}
