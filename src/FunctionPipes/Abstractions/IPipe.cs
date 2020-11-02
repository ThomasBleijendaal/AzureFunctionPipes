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
            IAsyncHttpStepProvider<HttpRequest, TReturn> provider);

        IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWithHttpRequest<TReturn>(
            HttpRequest httpRequest,
            ISyncHttpStepProvider<HttpRequest, TReturn> provider);

        IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWithHttpRequest<TReturn, TStepProvider>(
            HttpRequest httpRequest);

        IPipeElement<PipeContext, TInput, TReturn> StartWithActivity<TInput, TReturn>(
            TInput input,
            IAsyncActivityStepProvider<TInput, TReturn> provider);

        IPipeElement<PipeContext, TInput, TReturn> StartWithActivity<TInput, TReturn>(
            TInput input,
            ISyncActivityStepProvider<TInput, TReturn> provider);

        IPipeElement<PipeContext, TInput, TReturn> StartWithActivity<TInput, TReturn, TStepProvider>(
            TInput input);

        IPipeElement<QueuePipeContext, string, TReturn> StartWithQueueMessage<TReturn>(
            string queueMessage,
            IAsyncQueueStepProvider<string, TReturn> provider);

        IPipeElement<QueuePipeContext, string, TReturn> StartWithQueueMessage<TReturn>(
            string queueMessage,
            ISyncQueueStepProvider<string, TReturn> provider);

        IPipeElement<QueuePipeContext, string, TReturn> StartWithQueueMessage<TReturn, TStepProvider>(
            string queueMessage);

        IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWithTimer<TReturn>(
            TimerInfo timer,
            IAsyncTimerStepProvider<TimerInfo, TReturn> provider);

        IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWithTimer<TReturn>(
            TimerInfo timer,
            ISyncTimerStepProvider<TimerInfo, TReturn> provider);

        IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWithTimer<TReturn, TStepProvider>(
            TimerInfo timer);
    }
}
