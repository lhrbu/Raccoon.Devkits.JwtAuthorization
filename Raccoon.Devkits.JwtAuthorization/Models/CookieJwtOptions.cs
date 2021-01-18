namespace Raccoon.Devkits.JwtAuthorization.Models
{
    public class CookieJwtOptions
    {
        public bool? Enable {get;set;} = false;
        public AuthorizationRule[]? Rules {get;set;}
    }
}