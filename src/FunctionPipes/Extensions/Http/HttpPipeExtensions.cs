using System;
using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Elements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FunctionPipes.Extensions.Http
{
    public static class HttpPipeExtensions
    {
        public static IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWith<TReturn>(
            this HttpRequest httpRequest,
            IServiceProvider serviceProvider,
            IHttpStepProvider<HttpRequest, TReturn> provider)
        {
            var context = new HttpPipeContext(serviceProvider, httpRequest);

            return new StartElement<HttpPipeContext, HttpRequest, TReturn>(context, httpRequest, provider);
        }

        public static async Task<IActionResult> CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<HttpPipeContext, TInput, TInputForNextStep> element,
            IHttpFinalStepProvider<TInputForNextStep, IActionResult> provider)
        {
            var finalStep = new FinalElement<HttpPipeContext, TInputForNextStep, IActionResult>(
                element.Context, element.PreviousElements.Append(element), provider);

            return await finalStep.ResolveAsync();
        }
    }
}
