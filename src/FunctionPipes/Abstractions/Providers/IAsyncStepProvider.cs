using System.Threading.Tasks;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IAsyncStepProvider : IStepProvider
    {
        internal Task<object> InternalDoAsync(object context, object input);
    }

    public interface IAsyncStepProvider<TContext, TInput, TReturn> : IAsyncStepProvider
    {
        Task<TReturn> DoAsync(TContext context, TInput input);

        async Task<object> IAsyncStepProvider.InternalDoAsync(object context, object input)
        {
            return (await DoAsync((TContext)context, (TInput)input).ConfigureAwait(false))!;
        }
    }
}
