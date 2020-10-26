using System.Collections.Generic;

namespace FunctionPipes.Abstractions
{
    public interface IPipeElement
    {
        IStepProvider GenericProvider { get; }
    }

    public interface IPipeElement<TContext, TInput, TReturn> : IPipeElement
    {
        TContext Context { get; }
        IEnumerable<IPipeElement> PreviousElements { get; }
        IStepProvider<TContext, TInput, TReturn> Provider { get; }

        IStepProvider IPipeElement.GenericProvider => Provider;
    }
}
