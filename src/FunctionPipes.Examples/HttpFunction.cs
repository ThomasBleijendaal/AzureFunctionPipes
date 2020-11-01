using System.Threading.Tasks;
using System.Web.Http;
using FunctionPipes.Abstractions;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace FunctionPipes.Examples
{
    public class HttpFunction
    {
        private readonly IPipe _pipe;

        public HttpFunction(IPipe pipe)
        {
            _pipe = pipe;
        }

        [FunctionName("HttpFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            return await _pipe
                .StartWithHttpRequest<HttpRequest, Authenticate>(req)
                .DoNext<ExampleBody, SerializeBodyTo<ExampleBody>>()
                .DoNext<ExampleResult, CallToExternalService<ExampleBody, ExampleResult>>()
                .DoNext(async (context, input) =>
                {
                    await Task.Delay(1000);

                    return input;
                })
                .CompleteWithAsync<IActionResult, ReturnResult>();
        }

        public class ExampleBody
        {
            public string Content { get; set; } = default!;
        }

        public class ExampleResult
        {
            public string ConvertedContent { get; set; } = default!;
        }

        public class Authenticate : IHttpStepProvider<HttpRequest, HttpRequest>
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

                context.CachePipeOutput(json);

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
