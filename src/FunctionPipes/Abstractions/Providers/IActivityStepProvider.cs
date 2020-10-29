using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IActivityStepProvider<TInput, TReturn> : IStepProvider<PipeContext, TInput, TReturn>
    {

    }
}
