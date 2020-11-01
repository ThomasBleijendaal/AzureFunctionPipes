using System.Linq;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;

namespace FunctionPipes.Elements
{
    internal class StartElement<TContext, TInput, TReturn> : Element<TContext, TInput, TReturn>, IStartElement
        where TContext : PipeContext
    {
        public StartElement(
            TContext context,
            TInput input,
            IStepProvider<TContext, TInput, TReturn> provider) : base(context, Enumerable.Empty<IPipeElement>(), provider)
        {
            Input = input;
        }

        public TInput Input { get; }
        
        object IStartElement.Input => Input!;
        PipeContext IStartElement.Context => Context;
    }
}
