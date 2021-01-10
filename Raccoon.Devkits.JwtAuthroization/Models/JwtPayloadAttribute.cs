using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.Models
{
    public class JwtPayloadAttribute:Attribute
    {
        private readonly Dictionary<string, object> _requirements = new();
        public IReadOnlyDictionary<string, object> Requirements => _requirements;
        public string CookieName { get; }
        public JwtPayloadAttribute(string cookieName,params (string Key,object Value)[] requirements)
        {
            CookieName = cookieName;
            //if (keys.Length != values.Length)
            //{ throw new ArgumentException("keys don't match values!");}
            _requirements = requirements.ToDictionary(item=>item.Key,item=>item.Value);
        }
    }
}
