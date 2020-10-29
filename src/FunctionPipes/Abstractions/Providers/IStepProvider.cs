using System.Threading.Tasks;

namespace FunctionPipes.Abstractions.Providers
{
    // TODO: sync versions?
    public interface IStepProvider
    {
        internal Task<object> InternalDoAsync(object context, object input);
    }

    public interface IStepProvider<TContext, TInput, TReturn> : IStepProvider
    {
        Task<TReturn> DoAsync(TContext context, TInput input);

        async Task<object> IStepProvider.InternalDoAsync(object context, object input)
        {
            return (await DoAsync((TContext)context, (TInput)input).ConfigureAwait(false))!;
        }
    }
}
