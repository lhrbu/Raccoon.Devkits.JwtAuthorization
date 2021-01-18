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
using Castle.Core.Logging;
using Serilog;

namespace Raccoon.Devkits.JwtAuthroization.Models
{
    public class CookieJwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        
        public CookieJwtAuthorizationMiddleware(
            RequestDelegate next)
        { _next = next;}
        
        private IEnumerable<AuthorizationRule> GetAuthorizationRulesFromController(HttpContext context)
        {
            IEnumerable<CookieJwtPayloadRuleAttribute>? payloadAttributes = context.Features.Get<IEndpointFeature>()?
                .Endpoint?.Metadata?.Where(item => item is CookieJwtPayloadRuleAttribute)
                .Select(item => (item as CookieJwtPayloadRuleAttribute)!);
            if((payloadAttributes is null)||payloadAttributes.Count()==0) 
            { return Enumerable.Empty<AuthorizationRule>();}
            return payloadAttributes.Select(item=>item.AuthorizationRule with{PathPattern = context.Request.Path});
        }

        //private bool WildCardCompare(string value,string pattern)
        //{
        //    string regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        //    return Regex.IsMatch(value,regexPattern);
        //}

        private AuthorizationRule? SearchSpecificRuleInController(AuthorizationRule ruleInConfig,IEnumerable<AuthorizationRule> rulesInController)=>
            rulesInController.FirstOrDefault(item => new PathString(item.PathPattern)
                .StartsWithSegments(item.PathPattern != "/" ? new PathString(item.PathPattern) : new PathString(string.Empty)));
        
        private bool Authorize(HttpContext context,AuthorizationRule rule)
        {
            JwtEncodeService jwtEncodeService = (context.RequestServices.GetService(typeof(JwtEncodeService)) as JwtEncodeService)!;
            KeyValuePair<string, string> cookie = context.Request.Cookies
                   .FirstOrDefault(item => item.Key == rule.CookieName);
            if (cookie.Equals(default(KeyValuePair<string, string>))) { return false; }
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
                
                PathString currentPath = context.Request.Path;
                IEnumerable<AuthorizationRule> authorizationRulesFromConfig = options.CurrentValue.Rules?
                    .Where(item => currentPath.StartsWithSegments(
                        item.PathPattern!="/"?new PathString(item.PathPattern):new PathString(string.Empty)))??
                        Enumerable.Empty<AuthorizationRule>();

                IEnumerable<AuthorizationRule> authorizationRulesFromController = GetAuthorizationRulesFromController(context);

                bool authorizedFlag = true;
                foreach (AuthorizationRule ruleFromConfig in authorizationRulesFromConfig)
                {
                    authorizedFlag = Authorize(context, ruleFromConfig);
                    if (!authorizedFlag) 
                    {
                        AuthorizationRule? ruleFromController = SearchSpecificRuleInController(
                            ruleFromConfig, 
                            authorizationRulesFromController);
                        if (ruleFromController is not null) { authorizedFlag = Authorize(context,ruleFromController); }
                        if (!authorizedFlag)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return;
                        }
                    }
                }
                
                foreach (AuthorizationRule rule in authorizationRulesFromController)
                {
                    authorizedFlag = Authorize(context, rule);
                    if (!authorizedFlag) 
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }

                if (authorizedFlag) { await _next(context); }
            }catch(Exception exception){
                Log.Logger.Error("{exception}", exception);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
            
        }
    }
}
