using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IHttpStepProvider<TInput, TReturn> : IStepProvider<HttpPipeContext, TInput, TReturn>
    {
    }
}
