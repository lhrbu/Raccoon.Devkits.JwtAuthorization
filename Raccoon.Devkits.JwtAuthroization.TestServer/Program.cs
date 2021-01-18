using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.TestServer
{
    public class Program
    {
        public static bool StartsWithSegments(PathString? Value,PathString other, StringComparison comparisonType)
        {
            string text = Value ?? string.Empty;
            string text2 = other.Value ?? string.Empty;
            if (text.StartsWith(text2, comparisonType))
            {
                if (text.Length != text2.Length)
                {
                    return text[text2.Length] == '/';
                }

                return true;
            }

            return false;
        }
        public static void Main(string[] args)
        {
            //PathString v1 = new("/Weather/V1");
            //PathString v2 = new("/Weather");
            //var b =StartsWithSegments(v1, v2,StringComparison.OrdinalIgnoreCase);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
