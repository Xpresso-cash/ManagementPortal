using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ManagementPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(
                        options =>
                        {
                            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false)
                             .Build();
                            //AWSParamStore objParamStore = new AWSParamStore();
                            string sslCertificateName = "ManagementPortal.pfx";//objParamStore.GetValueFromParamStore(DataConstants.ParamStoreSSLCertificateName).Result;
                            string strPortNumber = "443";//objParamStore.GetValueFromParamStore(DataConstants.ParamStoreSSLServicePort).Result;
                            string strPassword = "Genx123!@#"; //objParamStore.GetValueFromParamStore(DataConstants.ParamStoreSSLCertificatePassword).Result;
                            int portNumber = Convert.ToInt32(strPortNumber);
                            options.Listen(IPAddress.Any, portNumber, listenOptions =>
                            {
                                listenOptions.UseHttps(sslCertificateName, strPassword);

                            });
                        }
                                      );
                    webBuilder.UseStartup<Startup>();
                });
    }
}
