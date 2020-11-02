using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface ISyncActivityStepProvider<TInput, TReturn> : ISyncStepProvider<PipeContext, TInput, TReturn>
    {

    }
}
