using System;
using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Elements;
using Microsoft.AspNetCore.Http;

namespace FunctionPipes.Extensions.Queue
{
    public static class QueuePipeExtensions
    {
        public static IPipeElement<QueuePipeContext, string, TReturn> StartWith<TReturn>(
            this string queueMessage,
            IServiceProvider serviceProvider,
            IQueueStepProvider<string, TReturn> provider)
        {
            var context = new QueuePipeContext(serviceProvider, queueMessage);

            return new StartElement<QueuePipeContext, string, TReturn>(context, queueMessage, provider);
        }

        public static async Task CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<QueuePipeContext, TInput, TInputForNextStep> element,
            IQueueFinalStepProvider<TInputForNextStep> provider)
        {
            var finalStep = new FinalElement<QueuePipeContext, TInputForNextStep, bool>(
                element.Context, element.PreviousElements.Append(element), provider);

            await finalStep.ResolveAsync();
        }
    }
}
