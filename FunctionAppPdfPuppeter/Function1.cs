using System;
using System.IO;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace FunctionAppPdfPuppeter
{
    public class Function1
    {
        private readonly AppInfo _appInfo;

        public Function1(AppInfo appInfo)
        {
            _appInfo = appInfo;
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                var source = File.ReadAllText(Path.Combine(context.FunctionAppDirectory, "index.html"));

                var template = Handlebars.Compile(source);

                var data = new
                {
                    userInfo = new
                    {
                        registeredNumber = 12345,
                        registeredEmail = "sadid@cefalo.com",
                        inceptionDate = "11/12/2021"
                    },
                    account = new
                    {
                        name = "Current Account",
                        currency = "FRW",
                        openingValue = 1000,
                        closingValue = 3124400
                    },
                    transactions = new[] {
                        new {
                            date = "10/11/12",
                            description = "",
                            time = "",
                            amount = 100,
                            isCredit = true
                        },
                        new {
                            date = "10/11/12",
                            description = "",
                            time = "example@gmail.com",
                            amount = 300,
                            isCredit = false
                        },
                        new {
                            date = "9/11/12",
                            description = "",
                            time = "example@gmail.com",
                            amount = 800,
                            isCredit = true
                        },
                        new {
                            date = "8/11/12",
                            description = "",
                            time = "example@gmail.com",
                            amount = 200,
                            isCredit = false
                        },
                        new {
                            date = "11/11/12",
                            description = "",
                            time = "example@gmail.com",
                            amount = 100,
                            isCredit = true
                        },
                    }

                };

                var result = template(data);

                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = _appInfo.BrowserExecutablePath
                });

                var page = await browser.NewPageAsync();
                //var result = await page.GetContentAsync();
                //await page.SetViewportAsync(new ViewPortOptions
                //{
                //    Width = 640,
                //    Height = 480,
                //    DeviceScaleFactor = 1
                //});

                await page.SetContentAsync(result);

                var stream = await page.PdfStreamAsync(new PdfOptions
                {
                    Format = PaperFormat.A4,
                    DisplayHeaderFooter = true,
                    MarginOptions = new MarginOptions
                    {
                        Top = "150px",
                        Bottom = "120px",

                    },
                    HeaderTemplate = @"<div style=""width: 100%;"">
            <img src=""data:image/gif;base64,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"" alt =""Company logo"" style=""width: 100%; max-width: 180px; height: 80px; float: right""/>
                    </div>",
                    FooterTemplate =
                        @"<div style=""border-top: solid 1px #bbb; width: 100%; height: 80px; font-size: 9px;
                                padding: 5px 5px 0; color: #bbb; position: relative;"">
                            <div style=""width: 100%; height: 75px; position: relative;"">
                                <div style=""position: absolute; left: 5px; top: 5px;"">
                                    <p class=""date""></p>
                                    <p>Customer service: 5090</p>
                                    <p><a href=""https://www.spenn.com"">www.spenn.com</a></p>
                                    
                                </div>
                                <div style=""position: absolute; right: 5px; top: 5px;"">
                                    <p>SPENN 2 RWANDA Limited</p>
                                    <p>Nyarugenge, Nyarugenge,</p>
                                    <p>Umujyi wa Kigali, RWANDA</p>
                                </div>
                                
                            </div>
                            <div style=""text-align: right;"">
                                <span>Page <span class="" pageNumber""></span> of <span class="" totalPages""></span></span>
                            </div>
                            
                        </div>"
                });

                await browser.CloseAsync();
                return new FileStreamResult(stream, "application/pdf");
                //return new OkObjectResult(responseMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public string GetBlob(string containerName, string fileName)
        {
            string connectionString = $"AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

            // Setup the connection to the storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Connect to the blob storage
            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
            // Connect to the blob container
            CloudBlobContainer container = serviceClient.GetContainerReference($"{containerName}");
            // Connect to the blob file
            CloudBlockBlob blob = container.GetBlockBlobReference($"{fileName}");
            // Get the blob file as text
            string contents = blob.DownloadTextAsync().Result;

            return contents;
        }
    }
}
