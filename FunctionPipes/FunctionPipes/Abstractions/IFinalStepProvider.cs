using System.Threading.Tasks;

namespace FunctionPipes.Abstractions
{
    public interface IFinalStepProvider<TContext, TInput>
    {
        Task FinalizeAsync(TContext context, TInput? input);
    }

    public interface IFinalStepProvider<TContext, TInput, TReturn>
    {
        Task<TReturn> FinalizeAsync(TContext context, TInput? input);
    }
}
