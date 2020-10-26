using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions
{
    public interface IQueueFinalStepProvider<TInput> : IFinalStepProvider<QueuePipeContext, TInput>
    {
    }
}
