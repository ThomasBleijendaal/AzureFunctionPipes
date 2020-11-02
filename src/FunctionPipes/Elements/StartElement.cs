using System;
using System.Linq;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;

namespace FunctionPipes.Elements
{
    internal class StartElement<TContext, TInput, TReturn> : Element<TContext, TInput, TReturn>, IStartElement
        where TContext : PipeContext
    {
        public static StartElement<TContext, TInput, TReturn> Create<TStepProvider>(TContext context, TInput input, TStepProvider provider)
        {
            if (provider is IAsyncStepProvider<TContext, TInput, TReturn> asyncProvider)
            {
                return new StartElement<TContext, TInput, TReturn>(
                    context,
                    input,
                    asyncProvider);
            }
            else if (provider is ISyncStepProvider<TContext, TInput, TReturn> syncProvider)
            {
                return new StartElement<TContext, TInput, TReturn>(
                    context,
                    input,
                    syncProvider);
            }
            else
            {
                throw new InvalidOperationException("Cannot determine if provider is sync or async.");
            }
        }

        public StartElement(
            TContext context,
            TInput input,
            IAsyncStepProvider<TContext, TInput, TReturn> provider) : base(context, Enumerable.Empty<IPipeElement>(), provider)
        {
            Input = input;
        }

        public StartElement(
            TContext context,
            TInput input,
            ISyncStepProvider<TContext, TInput, TReturn> provider) : base(context, Enumerable.Empty<IPipeElement>(), provider)
        {
            Input = input;
        }

        public TInput Input { get; }
        
        object IStartElement.Input => Input!;
        PipeContext IStartElement.Context => Context;
    }
}
