using System;
using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Elements;
using FunctionPipes.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionPipes
{
    public static class PipeExtensions
    {
        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            IAsyncStepProvider<TContext, TInputForNextStep, TReturn> provider)
            where TContext : PipeContext
        {
            return new Element<TContext, TInputForNextStep, TReturn>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);
        }

        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            ISyncStepProvider<TContext, TInputForNextStep, TReturn> provider)
            where TContext : PipeContext
        {
            return new Element<TContext, TInputForNextStep, TReturn>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);
        }

        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            Func<TContext, TInputForNextStep, Task<TReturn>> inlineMethod)
            where TContext : PipeContext
            => element.DoNext(new AsyncLambdaStepProvider<TContext, TInputForNextStep, TReturn>(inlineMethod));

        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            Func<TContext, TInputForNextStep, TReturn> inlineMethod)
            where TContext : PipeContext
            => element.DoNext(new SyncLambdaStepProvider<TContext, TInputForNextStep, TReturn>(inlineMethod));

        public static async Task<TReturn> CompleteWithAsync<TInput, TInputForNextStep, TReturn>(
            this IPipeElement<PipeContext, TInput, TInputForNextStep> element,
            IAsyncActivityStepProvider<TInputForNextStep?, TReturn> provider)
        {
            var finalStep = new FinalElement<PipeContext, TInputForNextStep, TReturn>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);

            return await finalStep.CompletePipeAsync();
        }

        public static async Task<IActionResult> CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<HttpPipeContext, TInput, TInputForNextStep> element,
            IAsyncHttpStepProvider<TInputForNextStep?, IActionResult> provider)
        {
            var finalStep = new FinalElement<HttpPipeContext, TInputForNextStep, IActionResult>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);

            return await finalStep.CompletePipeAsync();
        }

        public static async Task CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<QueuePipeContext, TInput, TInputForNextStep> element,
            IAsyncQueueStepProvider<TInputForNextStep?, bool> provider)
        {
            var finalStep = new FinalElement<QueuePipeContext, TInputForNextStep, bool>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);

            await finalStep.CompletePipeAsync();
        }

        public static async Task CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<TimerPipeContext, TInput, TInputForNextStep> element,
            IAsyncTimerStepProvider<TInputForNextStep?, bool> provider)
        {
            var finalStep = new FinalElement<TimerPipeContext, TInputForNextStep, bool>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);

            await finalStep.CompletePipeAsync();
        }

        internal static async Task<TReturn> InternalCompleteWithAsync<TStepProvider, TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element)
            where TStepProvider : IAsyncStepProvider<TContext, TInputForNextStep, TReturn>
            where TContext : PipeContext
        {
            var finalStep = new FinalElement<TContext, TInputForNextStep, TReturn>(
                element.Context,
                element.PreviousElements.Append(element),
                element.Context.ServiceProvider.GetRequiredService<TStepProvider>());

            return await finalStep.CompletePipeAsync();
        }
    }
}
