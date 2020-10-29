using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface ITimerStepProvider<TInput, TReturn> : IStepProvider<TimerPipeContext, TInput, TReturn>
    {
    }
}
