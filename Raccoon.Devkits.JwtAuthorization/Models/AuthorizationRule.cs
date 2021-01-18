namespace Raccoon.Devkits.JwtAuthorization.Models
{
    public record  AuthorizationRule
    {
        public string PathPattern {get;set;} = null!;
        public string CookieName { get;set;} = null!;
        public string RequiredHeader {get;set;} = null!;
        public object[] AllowedRange {get;set;} = null!;
    }
}