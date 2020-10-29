using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IQueueStepProvider<TInput, TReturn> : IStepProvider<QueuePipeContext, TInput, TReturn>
    {

    }
}
