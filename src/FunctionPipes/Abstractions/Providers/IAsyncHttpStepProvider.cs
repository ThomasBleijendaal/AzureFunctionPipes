using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IAsyncHttpStepProvider<TInput, TReturn> : IAsyncStepProvider<HttpPipeContext, TInput, TReturn>
    {
    }
}
