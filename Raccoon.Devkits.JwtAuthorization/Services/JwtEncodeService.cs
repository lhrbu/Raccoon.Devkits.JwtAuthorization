using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Raccoon.Devkits.JwtAuthroization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.Services
{
    public class JwtEncodeService
    {
        private readonly IJwtEncoder _encoder;
        private readonly IJwtDecoder _decoder;
        public JwtEncodeService()
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJsonSerializer jsonSerializer = new SystemTextJsonSerializer();
            IJwtValidator validator = new JwtValidator(jsonSerializer, new UtcDateTimeProvider());
            _encoder = new JwtEncoder(algorithm, jsonSerializer, urlEncoder);
            _decoder = new JwtDecoder(jsonSerializer, validator, urlEncoder, algorithm);
        }

        public string Encode(IDictionary<string, object> payload, string secret) =>_encoder.Encode(payload,secret);

        public IDictionary<string,object> Decode(string token, string secret) =>
            _decoder.DecodeToObject(token, secret, true);

       
        /// <exception cref="ArgumentException">Throw if token without exp field.</exception>
        /// <exception cref="TokenExpiredException"></exception>
        public string RefreshToken(string token,string secret,double timeExpand)
        {
            IDictionary<string, object> payload = Decode(token, secret);
            if (payload.ContainsKey("exp"))
            {
                double exp = Convert.ToDouble(payload["exp"]);
                if(exp< DateTimeOffset.Now.ToUnixTimeSeconds())
                { throw new TokenExpiredException($"{token} is expired already!");}
                payload["exp"] = DateTimeOffset.Now.ToUnixTimeSeconds() + timeExpand;
                return Encode(payload, secret);
            }
            else
            { throw new ArgumentException("Can't refresh token without exp key!", nameof(token));}
        }
    }
}
