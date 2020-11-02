using System.Collections.Generic;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Elements
{
    public interface IPipeElement
    {
        IAsyncStepProvider? GenericAsyncProvider { get; }

        ISyncStepProvider? GenericSyncProvider { get; }

        PipeContext GenericContext { get; }

        IEnumerable<IPipeElement> PreviousElements { get; }
    }

    public interface IPipeElement<TContext, TInput, TReturn> : IPipeElement
        where TContext : PipeContext
    {
        TContext Context { get; }
        IAsyncStepProvider<TContext, TInput, TReturn>? AsyncProvider { get; }

        ISyncStepProvider<TContext, TInput, TReturn>? SyncProvider { get; }

        IPipeElement<TContext, TReturn, TReturnOfNextStep> DoNext<TReturnOfNextStep, TStepProvider>();

        Task<TReturnOfNextStep> CompleteWithAsync<TReturnOfNextStep, TFinalStepProvider>()
            where TFinalStepProvider : IAsyncStepProvider<TContext, TReturn?, TReturnOfNextStep>;

        Task<TReturnOfNextStep> CompleteWith<TReturnOfNextStep, TFinalStepProvider>()
            where TFinalStepProvider : ISyncStepProvider<TContext, TReturn?, TReturnOfNextStep>;

        IAsyncStepProvider? IPipeElement.GenericAsyncProvider => AsyncProvider;
        ISyncStepProvider? IPipeElement.GenericSyncProvider => SyncProvider;

        PipeContext IPipeElement.GenericContext => Context;
    }
}
