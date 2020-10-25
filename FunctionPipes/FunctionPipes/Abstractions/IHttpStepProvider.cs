using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions
{
    public interface IHttpStepProvider<TInput, TReturn> : IStepProvider<HttpPipeContext, TInput, TReturn>
    {
    }
}
