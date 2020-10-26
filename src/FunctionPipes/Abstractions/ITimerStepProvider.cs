using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions
{
    public interface ITimerStepProvider<TInput, TReturn> : IStepProvider<TimerPipeContext, TInput, TReturn>
    {

    }
}
