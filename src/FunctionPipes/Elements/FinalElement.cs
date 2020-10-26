using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions;
using FunctionPipes.Contexts;

namespace FunctionPipes.Elements
{
    // TODO: remove this duplication
    public class FinalElement<TContext, TInput, TReturn>
        where TContext : PipeContext
    {
        public FinalElement(
            TContext context,
            IEnumerable<IPipeElement> previousElements,
            IFinalStepProvider<TContext, TInput, TReturn> provider)
        {
            Context = context;
            PreviousElements = previousElements;
            Provider = provider;
        }

        public TContext Context { get; }
        public IEnumerable<IPipeElement> PreviousElements { get; }
        public IFinalStepProvider<TContext, TInput, TReturn> Provider { get; }

        public async Task<TReturn> ResolveAsync()
        {
            var firstElement = (IStartElement)PreviousElements.First();

            var input = firstElement.Input;
            var context = firstElement.Context;

            foreach (var element in PreviousElements)
            {
                try
                {
                    var output = await element.GenericProvider.InternalDoAsync(context, input).ConfigureAwait(false);

                    input = output;
                }
                catch (Exception ex)
                {
                    Context.ThrownException = ex;

                    return await Provider.FinalizeAsync(Context, default).ConfigureAwait(false);
                }
            }

            return await Provider.FinalizeAsync(Context, (TInput)input).ConfigureAwait(false);
        }
    }

    public class FinalElement<TContext, TInput>
        where TContext : PipeContext
    {
        public FinalElement(
            TContext context,
            IEnumerable<IPipeElement> previousElements,
            IFinalStepProvider<TContext, TInput> provider)
        {
            Context = context;
            PreviousElements = previousElements;
            Provider = provider;
        }

        public TContext Context { get; }
        public IEnumerable<IPipeElement> PreviousElements { get; }
        public IFinalStepProvider<TContext, TInput> Provider { get; }

        public async Task ResolveAsync()
        {
            var firstElement = (IStartElement)PreviousElements.First();

            var input = firstElement.Input;
            var context = firstElement.Context;

            foreach (var element in PreviousElements)
            {
                try
                {
                    var output = await element.GenericProvider.InternalDoAsync(context, input).ConfigureAwait(false);

                    input = output;
                }
                catch (Exception ex)
                {
                    Context.ThrownException = ex;

                    await Provider.FinalizeAsync(Context, default).ConfigureAwait(false);
                }
            }

            await Provider.FinalizeAsync(Context, (TInput)input).ConfigureAwait(false);
        }
    }
}
