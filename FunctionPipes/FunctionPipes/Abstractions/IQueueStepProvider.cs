using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions
{
    public interface IQueueStepProvider<TInput, TReturn> : IStepProvider<QueuePipeContext, TInput, TReturn>
    {

    }
}
