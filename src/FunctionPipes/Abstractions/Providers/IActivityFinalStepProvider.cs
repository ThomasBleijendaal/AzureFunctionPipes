using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IActivityFinalStepProvider<TInput, TReturn> : IFinalStepProvider<PipeContext, TInput, TReturn>
    {
    }
}
