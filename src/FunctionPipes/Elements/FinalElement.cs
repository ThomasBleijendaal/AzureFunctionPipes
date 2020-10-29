using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Elements;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionPipes.Elements
{
    internal class FinalElement<TContext, TInput, TReturn>
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

            var cache = default(IMemoryCache);

            foreach (var element in PreviousElements)
            {
                try
                {
                    var output = await element.GenericProvider.InternalDoAsync(context, input).ConfigureAwait(false);

                    if (context.EnableCache && context.CacheFromThisStep && context.CacheKey != null)
                    {
                        context.CacheFromThisStep = false;

                        cache ??= context.ServiceProvider.GetRequiredService<IMemoryCache>();
                        if (cache.TryGetValue<TReturn>(context.CacheKey, out var cachedReturn))
                        {
                            return cachedReturn;
                        }
                    }

                    input = output;
                }
                catch (Exception ex)
                {
                    Context.ThrownException = ex;

                    return await Provider.FinalizeAsync(Context, default).ConfigureAwait(false);
                }
            }

            var result = await Provider.FinalizeAsync(Context, (TInput)input).ConfigureAwait(false);
            if (context.EnableCache && context.CacheKey != null && cache != null)
            {
                cache.Set(context.CacheKey, result, context.CacheLifeTime ?? new TimeSpan(0, 5, 0));
            }

            return result;
        }
    }
}
