using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions;
using FunctionPipes.Contexts;
using FunctionPipes.Elements;
using Microsoft.AspNetCore.Http;

namespace FunctionPipes.Extensions.Activity
{
    public static class ActivityPipeExtensions
    {
        public static IPipeElement<PipeContext, TInput, TReturn> StartWith<TInput, TReturn>(
            this TInput input,
            IActivityStepProvider<TInput, TReturn> provider)
        {
            var context = new PipeContext();

            return new StartElement<PipeContext, TInput, TReturn>(context, input, provider);
        }

        public static async Task<TReturn> CompleteWithAsync<TInput, TInputForNextStep, TReturn>(
            this IPipeElement<PipeContext, TInput, TInputForNextStep> element,
            IActivityFinalStepProvider<TInputForNextStep, TReturn> provider)
        {
            var finalStep = new FinalElement<PipeContext, TInputForNextStep, TReturn>(
                element.Context, element.PreviousElements.Append(element), provider);

            return await finalStep.ResolveAsync();
        }
    }
}
