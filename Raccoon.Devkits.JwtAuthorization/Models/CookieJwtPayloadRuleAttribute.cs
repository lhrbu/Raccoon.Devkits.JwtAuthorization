using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raccoon.Devkits.JwtAuthorization.Models;

namespace Raccoon.Devkits.JwtAuthroization.Models
{
    [AttributeUsage(AttributeTargets.All,AllowMultiple =true)]
    public class CookieJwtPayloadRuleAttribute:Attribute
    {
        public AuthorizationRule AuthorizationRule {get;}
        public CookieJwtPayloadRuleAttribute(string cookieName,string requiredHeader, params object[] allowedRange)
        {
           AuthorizationRule = new()
           {
               CookieName = cookieName,
               RequiredHeader = requiredHeader,
               AllowedRange = allowedRange
           };
        }
    }
}
