using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions
{
    public interface IHttpFinalStepProvider<TInput, TReturn> : IFinalStepProvider<HttpPipeContext, TInput, TReturn>
    {
    }
}
