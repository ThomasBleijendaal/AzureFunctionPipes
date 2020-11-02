using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface ISyncQueueStepProvider<TInput, TReturn> : ISyncStepProvider<QueuePipeContext, TInput, TReturn>
    {

    }
}
