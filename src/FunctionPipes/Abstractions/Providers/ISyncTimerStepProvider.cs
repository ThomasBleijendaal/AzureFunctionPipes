using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface ISyncTimerStepProvider<TInput, TReturn> : ISyncStepProvider<TimerPipeContext, TInput, TReturn>
    {
    }
}
