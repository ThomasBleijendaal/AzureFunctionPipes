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
            IHttpStepProvider<HttpRequest, TReturn> provider)
        {
            var context = new HttpPipeContext(_serviceProvider, httpRequest);

            return new StartElement<HttpPipeContext, HttpRequest, TReturn>(context, httpRequest, provider);
        }

        public IPipeElement<HttpPipeContext, HttpRequest, TReturn> StartWithHttpRequest<TReturn, TStepProvider>(
            HttpRequest httpRequest)
            where TStepProvider : IStepProvider<HttpPipeContext, HttpRequest, TReturn>
        {
            var context = new HttpPipeContext(_serviceProvider, httpRequest);

            return new StartElement<HttpPipeContext, HttpRequest, TReturn>(
                context, 
                httpRequest, 
                _serviceProvider.GetRequiredService<TStepProvider>());
        }

        public IPipeElement<PipeContext, TInput, TReturn> StartWithActivity<TInput, TReturn>(
            TInput input,
            IActivityStepProvider<TInput, TReturn> provider)
        {
            var context = new PipeContext(_serviceProvider);

            return new StartElement<PipeContext, TInput, TReturn>(context, input, provider);
        }

        public IPipeElement<PipeContext, TInput, TReturn> StartWithActivity<TInput, TReturn, TStepProvider>(
            TInput input)
            where TStepProvider : IStepProvider<PipeContext, TInput, TReturn>
        {
            var context = new PipeContext(_serviceProvider);

            return new StartElement<PipeContext, TInput, TReturn>(
                context,
                input,
                _serviceProvider.GetRequiredService<TStepProvider>());
        }

        public IPipeElement<QueuePipeContext, string, TReturn> StartWithQueueMessage<TReturn>(
            string queueMessage,
            IQueueStepProvider<string, TReturn> provider)
        {
            var context = new QueuePipeContext(_serviceProvider, queueMessage);

            return new StartElement<QueuePipeContext, string, TReturn>(context, queueMessage, provider);
        }

        public IPipeElement<QueuePipeContext, string, TReturn> StartWithQueueMessage<TReturn, TStepProvider>(
            string queueMessage)
            where TStepProvider : IStepProvider<QueuePipeContext, string, TReturn>
        {
            var context = new QueuePipeContext(_serviceProvider, queueMessage);

            return new StartElement<QueuePipeContext, string, TReturn>(
                context,
                queueMessage,
                _serviceProvider.GetRequiredService<TStepProvider>());
        }

        public IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWithTimer<TReturn>(
            TimerInfo timer,
            ITimerStepProvider<TimerInfo, TReturn> provider)
        {
            var context = new TimerPipeContext(_serviceProvider, timer);

            return new StartElement<TimerPipeContext, TimerInfo, TReturn>(context, timer, provider);
        }

        public IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWithTimer<TReturn, TStepProvider>(
            TimerInfo timer)
            where TStepProvider : IStepProvider<TimerPipeContext, TimerInfo, TReturn>
        {
            var context = new TimerPipeContext(_serviceProvider, timer);

            return new StartElement<TimerPipeContext, TimerInfo, TReturn>(
                context,
                timer,
                _serviceProvider.GetRequiredService<TStepProvider>());
        }
    }
}
