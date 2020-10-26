using System.Threading.Tasks;
using System.Web.Http;
using FunctionPipes.Abstractions;
using FunctionPipes.Contexts;
using FunctionPipes.Extensions;
using FunctionPipes.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace FunctionPipes.Examples
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
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            return await req
                .StartWith(_authenicate)
                .DoNext(_serializeBodyToExampleBody)
                .DoNext(_callToExternalService)
                .CompleteWithAsync(_returnResult);
        }

        public class ExampleBody
        {
            public string Content { get; set; } = default!;
        }

        public class ExampleResult
        {
            public string ConvertedContent { get; set; } = default!;
        }

        public class Authenicate : IHttpStepProvider<HttpRequest, HttpRequest>
        {
            public Task<HttpRequest> DoAsync(HttpPipeContext context, HttpRequest input)
            {
                return Task.FromResult(input);
            }
        }

        public class SerializeBodyTo<TModel> : IHttpStepProvider<HttpRequest, TModel>
            where TModel : class, new()
        {
            public async Task<TModel> DoAsync(HttpPipeContext context, HttpRequest input)
            {
                var json = await context.Request.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TModel>(json) ?? new TModel();
            }
        }

        public class CallToExternalService<TInputModel, TResultModel> : IHttpStepProvider<TInputModel, TResultModel>
            where TResultModel : class, new()
        {
            public Task<TResultModel> DoAsync(HttpPipeContext context, TInputModel input)
            {
                return Task.FromResult(JsonConvert.DeserializeObject<TResultModel>(JsonConvert.SerializeObject(input)));
            }
        }

        public class ReturnResult : IHttpFinalStepProvider<ExampleResult, IActionResult>
        {
            public Task<IActionResult> FinalizeAsync(HttpPipeContext context, ExampleResult? input)
            {
                if (input == null)
                {
                    return Task.FromResult(new BadRequestErrorMessageResult(context.ThrownException!.Message) as IActionResult);
                }

                return Task.FromResult(new OkObjectResult(input) as IActionResult);
            }
        }
    }
}
