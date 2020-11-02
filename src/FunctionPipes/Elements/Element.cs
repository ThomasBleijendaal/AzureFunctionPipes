using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionPipes.Elements
{
    internal class Element<TContext, TInput, TReturn> : IPipeElement<TContext, TInput, TReturn>
        where TContext : PipeContext
    {
        public static Element<TContext, TInput, TReturn> Create<TStepProvider>(TContext context, IEnumerable<IPipeElement> previousElements, TStepProvider provider)
        {
            if (provider is IAsyncStepProvider<TContext, TInput, TReturn> asyncProvider)
            {
                return new Element<TContext, TInput, TReturn>(
                    context,
                    previousElements,
                    asyncProvider);
            }
            else if (provider is ISyncStepProvider<TContext, TInput, TReturn> syncProvider)
            {
                return new Element<TContext, TInput, TReturn>(
                    context,
                    previousElements,
                    syncProvider);
            }
            else
            {
                throw new InvalidOperationException("Cannot determine if provider is sync or async.");
            }
        }

        public Element(
            TContext context,
            IEnumerable<IPipeElement> previousElements,
            IAsyncStepProvider<TContext, TInput, TReturn> provider)
        {
            Context = context;
            PreviousElements = previousElements;
            AsyncProvider = provider;
        }

        public Element(
            TContext context,
            IEnumerable<IPipeElement> previousElements,
            ISyncStepProvider<TContext, TInput, TReturn> provider)
        {
            Context = context;
            PreviousElements = previousElements;
            SyncProvider = provider;
        }

        public TContext Context { get; }
        public IEnumerable<IPipeElement> PreviousElements { get; }
        public IAsyncStepProvider<TContext, TInput, TReturn>? AsyncProvider { get; }
        public ISyncStepProvider<TContext, TInput, TReturn>? SyncProvider { get; }

        public IPipeElement<TContext, TReturn, TReturnOfNextStep> DoNext<TReturnOfNextStep, TStepProvider>()
        {
            return Element<TContext, TReturn, TReturnOfNextStep>.Create(
                Context, 
                PreviousElements.Append(this), 
                Context.ServiceProvider.GetRequiredService<TStepProvider>());
        }

        public async Task<TReturnOfNextStep> CompleteWithAsync<TReturnOfNextStep, TFinalStepProvider>()
            where TFinalStepProvider : IAsyncStepProvider<TContext, TReturn?, TReturnOfNextStep>
        {
            var finalStep = FinalElement<TContext, TReturn, TReturnOfNextStep>.Create(
                Context,
                PreviousElements.Append(this),
                Context.ServiceProvider.GetRequiredService<TFinalStepProvider>());

            return await finalStep.CompletePipeAsync();
        }

        public async Task<TReturnOfNextStep> CompleteWith<TReturnOfNextStep, TFinalStepProvider>()
            where TFinalStepProvider : ISyncStepProvider<TContext, TReturn?, TReturnOfNextStep>
        {
            var finalStep = FinalElement<TContext, TReturn, TReturnOfNextStep>.Create(
                Context,
                PreviousElements.Append(this),
                Context.ServiceProvider.GetRequiredService<TFinalStepProvider>());

            return await finalStep.CompletePipeAsync();
        }
    }
}
