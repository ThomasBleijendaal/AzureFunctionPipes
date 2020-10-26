using System.Collections.Generic;
using FunctionPipes.Abstractions;

namespace FunctionPipes.Elements
{
    public class Element<TContext, TInput, TReturn> : IPipeElement<TContext, TInput, TReturn>
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
    }
}
