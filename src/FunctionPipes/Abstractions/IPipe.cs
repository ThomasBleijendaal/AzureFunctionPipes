using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;

namespace FunctionPipes.Abstractions
{
    public interface IPipe
    {
        IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWithHttpRequest<TReturn>(
            HttpRequest httpRequest,
            IHttpStepProvider<HttpRequest, TReturn> provider);

        IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWithHttpRequest<TReturn, TStepProvider>(
            HttpRequest httpRequest)
            where TStepProvider : IStepProvider<HttpPipeContext, HttpRequest, TReturn>;

        IPipeElement<PipeContext, TInput, TReturn> StartWithActivity<TInput, TReturn>(
            TInput input,
            IActivityStepProvider<TInput, TReturn> provider);

        IPipeElement<QueuePipeContext, string, TReturn> StartWithQueueMessage<TReturn>(
            string queueMessage,
            IQueueStepProvider<string, TReturn> provider);

        IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWithTimer<TReturn>(
            TimerInfo timer,
            ITimerStepProvider<TimerInfo, TReturn> provider);
    }
}
