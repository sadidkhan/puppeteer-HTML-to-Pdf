using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp;

[assembly: FunctionsStartup(typeof(FunctionAppPdfPuppeter.Startup))]

namespace FunctionAppPdfPuppeter
{
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var bfOptions = new BrowserFetcherOptions();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                bfOptions.Path = Path.GetTempPath();
            }
            var bf = new BrowserFetcher(bfOptions);
            var a = bf.DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Result;
            bf.DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Wait();
            var info = new AppInfo
            {
                BrowserExecutablePath = bf.GetExecutablePath(BrowserFetcher.DefaultChromiumRevision)
            };

            builder.Services.AddSingleton(info);
        }
    }

    public class AppInfo
    {
        public string BrowserExecutablePath { get; set; }
        public int RazorPagesServerPort { get; set; }
    }
}
