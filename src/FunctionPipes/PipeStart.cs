using System;
using FunctionPipes.Abstractions;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Elements;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionPipes
{
    internal class PipeStart : IPipe
    {
        private readonly IServiceProvider _serviceProvider;

        public PipeStart(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWithHttpRequest<TReturn>(
            HttpRequest httpRequest,
            IAsyncHttpStepProvider<HttpRequest, TReturn> provider)
        {
            var context = new HttpPipeContext(_serviceProvider, httpRequest);

            return new StartElement<HttpPipeContext, HttpRequest, TReturn>(context, httpRequest, provider);
        }

        public IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWithHttpRequest<TReturn>(
            HttpRequest httpRequest,
            ISyncHttpStepProvider<HttpRequest, TReturn> provider)
        {
            var context = new HttpPipeContext(_serviceProvider, httpRequest);

            return new StartElement<HttpPipeContext, HttpRequest, TReturn>(context, httpRequest, provider);
        }

        public IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWithHttpRequest<TReturn, TStepProvider>(
            HttpRequest httpRequest)
        {
            var context = new HttpPipeContext(_serviceProvider, httpRequest);

            var provider = _serviceProvider.GetRequiredService<TStepProvider>();

            return StartElement<HttpPipeContext, HttpRequest, TReturn>.Create(context, httpRequest, provider);
        }

        public IPipeElement<PipeContext, TInput, TReturn> StartWithActivity<TInput, TReturn>(
            TInput input,
            IAsyncActivityStepProvider<TInput, TReturn> provider)
        {
            var context = new PipeContext(_serviceProvider);

            return new StartElement<PipeContext, TInput, TReturn>(context, input, provider);
        }

        public IPipeElement<PipeContext, TInput, TReturn> StartWithActivity<TInput, TReturn>(
            TInput input,
            ISyncActivityStepProvider<TInput, TReturn> provider)
        {
            var context = new PipeContext(_serviceProvider);

            return new StartElement<PipeContext, TInput, TReturn>(context, input, provider);
        }

        public IPipeElement<PipeContext, TInput, TReturn> StartWithActivity<TInput, TReturn, TStepProvider>(
            TInput input)
        {
            var context = new PipeContext(_serviceProvider);

            var provider = _serviceProvider.GetRequiredService<TStepProvider>();

            return StartElement<PipeContext, TInput, TReturn>.Create(context, input, provider);
        }

        public IPipeElement<QueuePipeContext, string, TReturn> StartWithQueueMessage<TReturn>(
            string queueMessage,
            IAsyncQueueStepProvider<string, TReturn> provider)
        {
            var context = new QueuePipeContext(_serviceProvider, queueMessage);

            return new StartElement<QueuePipeContext, string, TReturn>(context, queueMessage, provider);
        }

        public IPipeElement<QueuePipeContext, string, TReturn> StartWithQueueMessage<TReturn>(
            string queueMessage,
            ISyncQueueStepProvider<string, TReturn> provider)
        {
            var context = new QueuePipeContext(_serviceProvider, queueMessage);

            return new StartElement<QueuePipeContext, string, TReturn>(context, queueMessage, provider);
        }

        public IPipeElement<QueuePipeContext, string, TReturn> StartWithQueueMessage<TReturn, TStepProvider>(
            string queueMessage)
        {
            var context = new QueuePipeContext(_serviceProvider, queueMessage);

            var provider = _serviceProvider.GetRequiredService<TStepProvider>();

            return StartElement<QueuePipeContext, string, TReturn>.Create(context, queueMessage, provider);
        }

        public IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWithTimer<TReturn>(
            TimerInfo timer,
            IAsyncTimerStepProvider<TimerInfo, TReturn> provider)
        {
            var context = new TimerPipeContext(_serviceProvider, timer);

            return new StartElement<TimerPipeContext, TimerInfo, TReturn>(context, timer, provider);
        }

        public IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWithTimer<TReturn>(
            TimerInfo timer,
            ISyncTimerStepProvider<TimerInfo, TReturn> provider)
        {
            var context = new TimerPipeContext(_serviceProvider, timer);

            return new StartElement<TimerPipeContext, TimerInfo, TReturn>(context, timer, provider);
        }

        public IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWithTimer<TReturn, TStepProvider>(
            TimerInfo timer)
        {
            var context = new TimerPipeContext(_serviceProvider, timer);

            var provider = _serviceProvider.GetRequiredService<TStepProvider>();

            return StartElement<TimerPipeContext, TimerInfo, TReturn>.Create(context, timer, provider);
        }
    }
}
