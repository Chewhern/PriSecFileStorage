using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;

namespace PriSecFileStorageAPI
{
    public class Program
    {
        public static String CertPath;
        public static String Password;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    SetPassword();
                    SetPath();
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Listen(IPAddress.Parse("0.0.0.0"), 5001,
                            listenOptions =>
                            {
                                listenOptions.UseHttps(CertPath,
                                    Password);
                            });
                    });
                    webBuilder.UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = 8388608;
                    });
                });

        public static void SetPassword()
        {
            StreamReader MyStreamReader = new StreamReader("{Path to X509 Certificate Password}");
            Password = MyStreamReader.ReadLine();
            MyStreamReader.Close();
        }

        public static void SetPath()
        {
            StreamReader MyStreamReader = new StreamReader("{Path to X509 Certificate}");
            CertPath = MyStreamReader.ReadLine();
            MyStreamReader.Close();
        }
    }
}
