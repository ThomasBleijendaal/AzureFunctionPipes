using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface ITimerFinalStepProvider<TInput> : IFinalStepProvider<TimerPipeContext, TInput, bool>
    {
    }
}
