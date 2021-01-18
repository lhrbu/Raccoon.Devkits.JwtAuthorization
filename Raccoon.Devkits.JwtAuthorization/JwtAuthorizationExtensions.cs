using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raccoon.Devkits.JwtAuthorization.Models;
using Raccoon.Devkits.JwtAuthroization.Models;
using Raccoon.Devkits.JwtAuthroization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthorization
{
    public static class JwtAuthorizationExtensions
    {
        public static IServiceCollection AddJwtAuthorization(this IServiceCollection services,IConfiguration configuration)
        {
            services.TryAddTransient<JwtEncodeService>();
            services.TryAddTransient<CookieJwtEncoder>();
            services.Configure<CookieJwtOptions>(configuration.GetSection(nameof(CookieJwtOptions)));
            return services;
        }
        public static IApplicationBuilder UseCookieJwtAuthorization(this IApplicationBuilder app) =>
            app.UseMiddleware<CookieJwtAuthorizationMiddleware>();
    }
}
