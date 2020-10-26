using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions
{
    public interface IActivityFinalStepProvider<TInput, TReturn> : IFinalStepProvider<PipeContext, TInput, TReturn>
    {
    }
}
