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

namespace FunctionPipes.Extensions
{
    public static class PipeExtensions
    {
        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            IStepProvider<TContext, TInputForNextStep, TReturn> provider)
            where TContext : PipeContext
        {
            return new Element<TContext, TInputForNextStep, TReturn>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);
        }

        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            Type providerType)
            where TContext : PipeContext
            => element.DoNext(
                element.Context.ServiceProvider.GetRequiredService(providerType) as IStepProvider<TContext, TInputForNextStep, TReturn>
                ?? throw new InvalidOperationException("Cannot find given type."));
        
        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            Func<TContext, TInputForNextStep, Task<TReturn>> inlineMethod)
            where TContext : PipeContext
            => element.DoNext(new LambdaStepProvider<TContext, TInputForNextStep, TReturn>(inlineMethod));

        internal static IPipeElement<TContext, TInputForNextStep, TReturn> InternalDoNext<TStepProvider, TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element)
            where TStepProvider : IStepProvider<TContext, TInputForNextStep, TReturn>
            where TContext : PipeContext
        {
            return new Element<TContext, TInputForNextStep, TReturn>(
                element.Context,
                element.PreviousElements.Append(element),
                element.Context.ServiceProvider.GetRequiredService<TStepProvider>());


            //var genericStepProviderInterface = stepProvider.GetType().GetInterface(typeof(IStepProvider<,,>).Name)
            //    ?? throw new InvalidOperationException("Cannot find the IStepProvider<,,> interface.");

            //var elementType = typeof(Element<,,>).MakeGenericType(genericStepProviderInterface.GenericTypeArguments);

            //return Activator.CreateInstance(
            //    elementType,
            //    element.GenericContext,
            //    element.PreviousElements.Append(element),
            //    stepProvider) as IPipeElement<TContext, TInputForNextStep, TReturn>
            //    ?? throw new InvalidCastException("Cannot create pipe element for step.");
        }

        public static async Task<TReturn> CompleteWithAsync<TInput, TInputForNextStep, TReturn>(
            this IPipeElement<PipeContext, TInput, TInputForNextStep> element,
            IActivityFinalStepProvider<TInputForNextStep, TReturn> provider)
        {
            var finalStep = new FinalElement<PipeContext, TInputForNextStep, TReturn>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);

            return await finalStep.CompletePipeAsync();
        }

        public static async Task<IActionResult> CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<HttpPipeContext, TInput, TInputForNextStep> element,
            IHttpFinalStepProvider<TInputForNextStep, IActionResult> provider)
        {
            var finalStep = new FinalElement<HttpPipeContext, TInputForNextStep, IActionResult>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);

            return await finalStep.CompletePipeAsync();
        }

        public static async Task CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<QueuePipeContext, TInput, TInputForNextStep> element,
            IQueueFinalStepProvider<TInputForNextStep> provider)
        {
            var finalStep = new FinalElement<QueuePipeContext, TInputForNextStep, bool>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);

            await finalStep.CompletePipeAsync();
        }

        public static async Task CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<TimerPipeContext, TInput, TInputForNextStep> element,
            ITimerFinalStepProvider<TInputForNextStep> provider)
        {
            var finalStep = new FinalElement<TimerPipeContext, TInputForNextStep, bool>(
                element.Context,
                element.PreviousElements.Append(element),
                provider);

            await finalStep.CompletePipeAsync();
        }

        internal static async Task<TReturn> InternalCompleteWithAsync<TStepProvider, TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element)
            where TStepProvider : IFinalStepProvider<TContext, TInputForNextStep, TReturn>
            where TContext : PipeContext
        {
            var finalStep = new FinalElement<TContext, TInputForNextStep, TReturn>(
                element.Context,
                element.PreviousElements.Append(element),
                element.Context.ServiceProvider.GetRequiredService<TStepProvider>());

            return await finalStep.CompletePipeAsync();

            //var stepProvider = ;



            //var genericStepProviderInterface = stepProvider.GetType().GetInterface(typeof(IStepProvider<,,>).Name)
            //    ?? throw new InvalidOperationException("Cannot find the IFinalStepProvider<,,> interface.");

            //var elementType = typeof(Element<,,>).MakeGenericType(genericStepProviderInterface.GenericTypeArguments);

            //var finalStep = Activator.CreateInstance(
            //    elementType,
            //    element.GenericContext,
            //    element.PreviousElements.Append(element),
            //    stepProvider) as FinalElement<TContext, TInputForNextStep, TReturn>
            //    ?? throw new InvalidCastException("Cannot create pipe element for step.");

            //return await finalStep.CompletePipeAsync();



            //var finalStep = new FinalElement<TimerPipeContext, TInputForNextStep, bool>(
            //    element.Context,
            //    element.PreviousElements.Append(element),
            //    provider);

            //await finalStep.CompletePipeAsync();
        }
    }
}
