using System.Threading.Tasks;

namespace FunctionPipes.Abstractions.Providers
{
    public interface IFinalStepProvider<TContext, TInput, TReturn>
    {
        Task<TReturn> FinalizeAsync(TContext context, TInput? input);
    }
}
