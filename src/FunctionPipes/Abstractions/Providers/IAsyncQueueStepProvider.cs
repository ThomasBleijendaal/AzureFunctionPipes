using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IAsyncQueueStepProvider<TInput, TReturn> : IAsyncStepProvider<QueuePipeContext, TInput, TReturn>
    {

    }
}
