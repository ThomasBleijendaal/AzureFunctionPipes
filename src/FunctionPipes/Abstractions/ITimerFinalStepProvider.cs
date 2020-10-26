using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions
{
    public interface ITimerFinalStepProvider<TInput> : IFinalStepProvider<TimerPipeContext, TInput>
    {
    }
}
