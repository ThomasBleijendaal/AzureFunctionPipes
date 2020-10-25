using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions;
using FunctionPipes.Contexts;
using FunctionPipes.Elements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;

namespace FunctionPipes
{
    // TODO: move these to namespaces
    public static class Pipes
    {
        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            IStepProvider<TContext, TInputForNextStep, TReturn> provider)
        {
            return new Element<TContext, TInputForNextStep, TReturn>(
                element.Context, element.PreviousElements.Append(element), provider);
        }
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

        public static async Task<IActionResult> CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<HttpPipeContext, TInput, TInputForNextStep> element,
            IHttpFinalStepProvider<TInputForNextStep, IActionResult> provider)
        {
            var finalStep = new FinalElement<HttpPipeContext, TInputForNextStep, IActionResult>(
                element.Context, element.PreviousElements.Append(element), provider);

            return await finalStep.ResolveAsync();
        }
    }

    public static class TimerPipes
    {
        public static IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWith<TReturn>(
            this TimerInfo timer,
            ITimerStepProvider<TimerInfo, TReturn> provider)
        {
            var context = new TimerPipeContext(timer);

            return new StartElement<TimerPipeContext, TimerInfo, TReturn>(context, timer, provider);
        }

        public static async Task CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<TimerPipeContext, TInput, TInputForNextStep> element,
            ITimerFinalStepProvider<TInputForNextStep> provider)
        {
            var finalStep = new FinalElement<TimerPipeContext, TInputForNextStep>(
                element.Context, element.PreviousElements.Append(element), provider);

            await finalStep.ResolveAsync();
        }
    }

    public static class QueuePipes
    {
        public static IPipeElement<QueuePipeContext, string, TReturn> StartWith<TReturn>(
            this string queueMessage,
            IQueueStepProvider<string, TReturn> provider)
        {
            var context = new QueuePipeContext(queueMessage);

            return new StartElement<QueuePipeContext, string, TReturn>(context, queueMessage, provider);
        }

        public static async Task CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<QueuePipeContext, TInput, TInputForNextStep> element,
            IQueueFinalStepProvider<TInputForNextStep> provider)
        {
            var finalStep = new FinalElement<QueuePipeContext, TInputForNextStep>(
                element.Context, element.PreviousElements.Append(element), provider);

            await finalStep.ResolveAsync();
        }
    }
}
