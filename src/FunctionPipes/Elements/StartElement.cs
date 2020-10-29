using System.Collections.Generic;
using System.Linq;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;

namespace FunctionPipes.Elements
{
    internal class StartElement<TContext, TInput, TReturn> : IPipeElement<TContext, TInput, TReturn>, IStartElement
        where TContext : PipeContext
    {
        public StartElement(
            TContext context,
            TInput input,
            IStepProvider<TContext, TInput, TReturn> provider)
        {
            Context = context;
            Input = input;
            PreviousElements = Enumerable.Empty<IPipeElement>();
            Provider = provider;
        }

        public TContext Context { get; }
        public TInput Input { get; }
        public IEnumerable<IPipeElement> PreviousElements { get; }
        public IStepProvider<TContext, TInput, TReturn> Provider { get; }

        object IStartElement.Input => Input!;
        PipeContext IStartElement.Context => Context;
    }
}
