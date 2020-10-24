using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionPipes
{
    public class HttpFunction
    {
        private readonly Authenicate _authenicate;
        private readonly SerializeBodyTo<ExampleBody> _serializeBodyToExampleBody;
        private readonly CallToExternalService<ExampleBody, ExampleResult> _callToExternalService;
        private readonly ReturnResult _returnResult;

        public HttpFunction(
            Authenicate authenicate,
            SerializeBodyTo<ExampleBody> serializeBodyToExampleBody,
            CallToExternalService<ExampleBody, ExampleResult> callToExternalService,
            ReturnResult returnResult)
        {
            _authenicate = authenicate;
            _serializeBodyToExampleBody = serializeBodyToExampleBody;
            _callToExternalService = callToExternalService;
            _returnResult = returnResult;
        }

        [FunctionName("HttpFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            // want to do:
            // - validate authentication
            // - serialize body
            // - call to external service
            // - handle common exceptions

            // important:
            // - short circuit to end when exceptions
            // - 

            return await req
                .StartWith(_authenicate)
                .DoNext(_serializeBodyToExampleBody)
                .DoNext(_callToExternalService)
                .EndWithAsync(_returnResult);

            //log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //return new OkObjectResult(responseMessage);
        }
    }

    public class ExampleBody
    {
        public string Content { get; set; }
    }

    public class ExampleResult
    {
        public string ConvertedContent { get; set; }
    }
}
