using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IAsyncTimerStepProvider<TInput, TReturn> : IAsyncStepProvider<TimerPipeContext, TInput, TReturn>
    {
    }
}
