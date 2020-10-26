using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions
{
    public interface IActivityStepProvider<TInput, TReturn> : IStepProvider<PipeContext, TInput, TReturn>
    {

    }
}
