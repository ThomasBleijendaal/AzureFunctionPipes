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
        public static FinalElement<TContext, TInput, TReturn> Create<TStepProvider>(TContext context, IEnumerable<IPipeElement> previousElements, TStepProvider provider)
        {
            if (provider is IAsyncStepProvider<TContext, TInput?, TReturn> asyncProvider)
            {
                return new FinalElement<TContext, TInput, TReturn>(
                    context,
                    previousElements,
                    asyncProvider);
            }
            else if (provider is ISyncStepProvider<TContext, TInput?, TReturn> syncProvider)
            {
                return new FinalElement<TContext, TInput, TReturn>(
                    context,
                    previousElements,
                    syncProvider);
            }
            else
            {
                throw new InvalidOperationException("Cannot determine if provider is sync or async.");
            }
        }

        public FinalElement(
            TContext context,
            IEnumerable<IPipeElement> previousElements,
            IAsyncStepProvider<TContext, TInput?, TReturn> provider)
        {
            Context = context;
            PreviousElements = previousElements;
            AsyncProvider = provider;
        }

        public FinalElement(
            TContext context,
            IEnumerable<IPipeElement> previousElements,
            ISyncStepProvider<TContext, TInput?, TReturn> provider)
        {
            Context = context;
            PreviousElements = previousElements;
            SyncProvider = provider;
        }

        public TContext Context { get; }
        public IEnumerable<IPipeElement> PreviousElements { get; }
        public IAsyncStepProvider<TContext, TInput?, TReturn>? AsyncProvider { get; }
        public ISyncStepProvider<TContext, TInput?, TReturn>? SyncProvider { get; }

        public async Task<TReturn> CompletePipeAsync()
        {
            var firstElement = (IStartElement)PreviousElements.First();

            var input = firstElement.Input;
            var context = firstElement.Context;

            var cache = default(IMemoryCache);

            foreach (var element in PreviousElements)
            {
                try
                {
                    var output = element.GenericSyncProvider != null 
                        ? element.GenericSyncProvider.InternalDo(context, input)
                        : element.GenericAsyncProvider != null 
                        ? await element.GenericAsyncProvider.InternalDoAsync(context, input).ConfigureAwait(false)
                        : throw new InvalidOperationException("No provider set for step.");

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

                    return SyncProvider != null
                        ? SyncProvider.Do(Context, default)
                        : AsyncProvider != null
                        ? await AsyncProvider.DoAsync(Context, default).ConfigureAwait(false)
                        : throw new InvalidOperationException("No provider set for final step.");
                }
            }

            var result = SyncProvider != null
                ? SyncProvider.Do(Context, (TInput)input)
                : AsyncProvider != null
                ? await AsyncProvider.DoAsync(Context, (TInput)input).ConfigureAwait(false)
                : throw new InvalidOperationException("No provider set for final step.");

            if (context.EnableCache && context.CacheKey != null && cache != null)
            {
                cache.Set(context.CacheKey, result, context.CacheLifeTime ?? new TimeSpan(0, 5, 0));
            }

            return result;
        }
    }
}
