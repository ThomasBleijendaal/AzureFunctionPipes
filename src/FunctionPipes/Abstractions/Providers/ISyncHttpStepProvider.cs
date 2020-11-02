using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface ISyncHttpStepProvider<TInput, TReturn> : ISyncStepProvider<HttpPipeContext, TInput, TReturn>
    {
    }
}
