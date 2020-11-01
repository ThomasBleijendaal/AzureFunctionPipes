using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionPipes.Elements
{
    internal class Element<TContext, TInput, TReturn> : IPipeElement<TContext, TInput, TReturn>
        where TContext : PipeContext
    {
        public Element(
            TContext context,
            IEnumerable<IPipeElement> previousElements,
            IStepProvider<TContext, TInput, TReturn> provider)
        {
            Context = context;
            PreviousElements = previousElements;
            Provider = provider;
        }

        public TContext Context { get; }
        public IEnumerable<IPipeElement> PreviousElements { get; }
        public IStepProvider<TContext, TInput, TReturn> Provider { get; }

        public IPipeElement<TContext, TReturn, TReturnOfNextStep> DoNext<TReturnOfNextStep, TStepProvider>()
            where TStepProvider : IStepProvider<TContext, TReturn, TReturnOfNextStep>
        {
            return this.InternalDoNext<TStepProvider, TContext, TInput, TReturn, TReturnOfNextStep>();
        }

        public async Task<TReturnOfNextStep> CompleteWithAsync<TReturnOfNextStep, TFinalStepProvider>()
            where TFinalStepProvider : IFinalStepProvider<TContext, TReturn, TReturnOfNextStep>
        {
            var finalStep = new FinalElement<TContext, TReturn, TReturnOfNextStep>(
                Context,
                PreviousElements.Append(this),
                Context.ServiceProvider.GetRequiredService<TFinalStepProvider>());

            return await finalStep.CompletePipeAsync();
        }
    }
}
