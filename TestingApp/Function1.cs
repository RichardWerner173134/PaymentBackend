using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TestingApp
{
    public static class Function1
    {
        [FunctionName(nameof(HelloWorldTrigger))]
        public static HttpResponseMessage HelloWorldTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            List<CustomResponseObject> response = new()
            {
                new CustomResponseObject()
                {
                    Message = "some static message from backend",
                    Number = 42
                },
                new CustomResponseObject()
                {
                    Message = "some other message",
                    Number = 69
                }
            };

            var result = JsonConvert.SerializeObject(response);


            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(result, Encoding.UTF8, "application/json")
            };
        }
    }

    class CustomResponseObject
    {
        public string Message { get; set; }
        public long Number { get; set; }
    }
}
