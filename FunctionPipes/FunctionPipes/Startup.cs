using FunctionPipes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace FunctionPipes
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<Authenicate>();
            builder.Services.AddSingleton<SerializeBodyTo<ExampleBody>>();
            builder.Services.AddSingleton<CallToExternalService<ExampleBody, ExampleResult>>();
            builder.Services.AddSingleton<ReturnResult>();
        }
    }

    public interface IPipeElement
    {
        IStepProvider GenericProvider { get; }
    }

    public interface IStartElement
    {
        object Input { get; }
        PipeContext Context { get; }
    }

    public interface IPipeElement<TContext, TInput, TReturn> : IPipeElement
    {
        TContext Context { get; }
        IEnumerable<IPipeElement> PreviousElements { get; }
        IStepProvider<TContext, TInput, TReturn> Provider { get; }

        IStepProvider IPipeElement.GenericProvider => Provider;
    }

    public interface IStepProvider
    {
        internal Task<object> InternalDoAsync(object context, object input);
    }


    public interface IStepProvider<TContext, TInput, TReturn> : IStepProvider
    {
        Task<TReturn> DoAsync(TContext context, TInput input);

        async Task<object> IStepProvider.InternalDoAsync(object context, object input)
        {
            return await DoAsync((TContext)context, (TInput)input).ConfigureAwait(false);
        }
    }

    public interface IFinalStepProvider<TContext, TInput, TReturn>
    {
        Task<TReturn> FinalizeAsync(TContext context, TInput? input);
    }

    public interface IHttpStepProvider<TInput, TReturn> : IStepProvider<HttpPipeContext, TInput, TReturn>
    {
    }

    public interface IHttpFinalStepProvider<TInput, TReturn> : IFinalStepProvider<HttpPipeContext, TInput, TReturn>
    {
    }

    public static class Pipes
    {
        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            IStepProvider<TContext, TInputForNextStep, TReturn> provider)
        {
            return new Element<TContext, TInputForNextStep, TReturn>(element.Context, element.PreviousElements.Append(element), provider);
        }
    }

    public class PipeContext
    {
        public Exception? ThrownException { get; set; }
    }

    public sealed class HttpPipeContext : PipeContext
    {
        public HttpPipeContext(HttpRequest request)
        {
            Request = request;
        }

        public HttpRequest Request { get; }
    }

    public static class HttpPipes
    {
        public static IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWith<TReturn>(
            this HttpRequest httpRequest,
            IHttpStepProvider<HttpRequest, TReturn> provider)
        {
            var context = new HttpPipeContext(httpRequest);

            return new StartElement<HttpPipeContext, HttpRequest, TReturn>(context, httpRequest, provider);
        }

        public static async Task<IActionResult> EndWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<HttpPipeContext, TInput, TInputForNextStep> element,
            IFinalStepProvider<HttpPipeContext, TInputForNextStep, IActionResult> provider)
        {
            var finalStep = new FinalElement<HttpPipeContext, TInputForNextStep, IActionResult>(element.Context, element.PreviousElements.Append(element), provider);

            return await finalStep.ResolveAsync();
        }
    }

    public class StartElement<TContext, TInput, TReturn> : IPipeElement<TContext, TInput, TReturn>, IStartElement
        where TContext : PipeContext
    {
        public StartElement(
            TContext context,
            TInput input,
            IStepProvider<TContext, TInput, TReturn> provider)
        {
            Context = context;
            Input = input;
            PreviousElements = Enumerable.Empty<IPipeElement>();
            Provider = provider;
        }

        public TContext Context { get; }
        public TInput Input { get; }
        public IEnumerable<IPipeElement> PreviousElements { get; }
        public IStepProvider<TContext, TInput, TReturn> Provider { get; }

        object IStartElement.Input => Input!;
        PipeContext IStartElement.Context => Context;
    }

    public class Element<TContext, TInput, TReturn> : IPipeElement<TContext, TInput, TReturn>
    {
        public Element(
            TContext context,
            IEnumerable<IPipeElement> previousElements,
            IStepProvider<TContext, TInput, TReturn> provider)
        {
            Context = context;
            PreviousElements = previousElements;
            Provider = provider;
        }

        public TContext Context { get; }
        public IEnumerable<IPipeElement> PreviousElements { get; }
        public IStepProvider<TContext, TInput, TReturn> Provider { get; }
    }

    public class FinalElement<TContext, TInput, TReturn>
        where TContext : PipeContext
    {
        public FinalElement(
            TContext context,
            IEnumerable<IPipeElement> previousElements,
            IFinalStepProvider<TContext, TInput, TReturn> provider)
        {
            Context = context;
            PreviousElements = previousElements;
            Provider = provider;
        }

        public TContext Context { get; }
        public IEnumerable<IPipeElement> PreviousElements { get; }
        public IFinalStepProvider<TContext, TInput, TReturn> Provider { get; }

        public async Task<TReturn> ResolveAsync()
        {
            var firstElement = (IStartElement)PreviousElements.First();

            var input = firstElement.Input;
            var context = firstElement.Context;

            foreach (var element in PreviousElements)
            {
                try
                {
                    var output = await element.GenericProvider.InternalDoAsync(context, input).ConfigureAwait(false);

                    input = output;
                }
                catch (Exception ex)
                {
                    Context.ThrownException = ex;

                    return await Provider.FinalizeAsync(Context, default).ConfigureAwait(false);
                }
            }

            return await Provider.FinalizeAsync(Context, (TInput)input).ConfigureAwait(false);
        }
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
        public Task<TModel> DoAsync(HttpPipeContext context, HttpRequest input)
        {
            return Task.FromResult(new TModel());
        }
    }

    public class CallToExternalService<TInputModel, TResultModel> : IHttpStepProvider<TInputModel, TResultModel>
        where TResultModel : class, new()
    {
        public Task<TResultModel> DoAsync(HttpPipeContext context, TInputModel input)
        {
            return Task.FromResult(new TResultModel());
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
