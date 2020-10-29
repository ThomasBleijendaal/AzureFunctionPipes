using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IQueueFinalStepProvider<TInput> : IFinalStepProvider<QueuePipeContext, TInput, bool>
    {
    }
}
