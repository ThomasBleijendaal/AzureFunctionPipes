using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IAsyncActivityStepProvider<TInput, TReturn> : IAsyncStepProvider<PipeContext, TInput, TReturn>
    {

    }
}
