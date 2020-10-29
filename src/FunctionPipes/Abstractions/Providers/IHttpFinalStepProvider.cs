using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IHttpFinalStepProvider<TInput, TReturn> : IFinalStepProvider<HttpPipeContext, TInput, TReturn>
    {
    }
}
