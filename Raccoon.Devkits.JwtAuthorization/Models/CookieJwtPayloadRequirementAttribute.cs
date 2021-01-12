using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.Models
{
    [AttributeUsage(AttributeTargets.All,AllowMultiple =true)]
    public class CookieJwtPayloadRequirementAttribute:Attribute
    {
        public string Key { get; }
        public object [] Values { get; }
        public string CookieName { get; }
        public CookieJwtPayloadRequirementAttribute(string cookieName,string key, params object[] values)
        {
            CookieName = cookieName;
            Key = key;
            Values = values;
        }
    }
}
