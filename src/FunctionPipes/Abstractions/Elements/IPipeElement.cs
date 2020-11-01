using System.Collections.Generic;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Elements
{
    public interface IPipeElement
    {
        IStepProvider GenericProvider { get; }

        PipeContext GenericContext { get; }

        IEnumerable<IPipeElement> PreviousElements { get; }
    }

    public interface IPipeElement<TContext, TInput, TReturn> : IPipeElement
        where TContext : PipeContext
    {
        TContext Context { get; }
        IStepProvider<TContext, TInput, TReturn> Provider { get; }

        IPipeElement<TContext, TReturn, TReturnOfNextStep> DoNext<TReturnOfNextStep, TStepProvider>()
            where TStepProvider : IStepProvider<TContext, TReturn, TReturnOfNextStep>;

        Task<TReturnOfNextStep> CompleteWithAsync<TReturnOfNextStep, TFinalStepProvider>()
            where TFinalStepProvider : IFinalStepProvider<TContext, TReturn, TReturnOfNextStep>;

        IStepProvider IPipeElement.GenericProvider => Provider;
        PipeContext IPipeElement.GenericContext => Context;
    }
}
