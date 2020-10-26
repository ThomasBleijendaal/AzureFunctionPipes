using System.Linq;
using System.Threading.Tasks;
using FunctionPipes.Abstractions;
using FunctionPipes.Contexts;
using FunctionPipes.Elements;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;

namespace FunctionPipes.Extensions.Timer
{
    public static class TimerPipeExtensions
    {
        public static IPipeElement<TimerPipeContext, TimerInfo, TReturn> StartWith<TReturn>(
            this TimerInfo timer,
            ITimerStepProvider<TimerInfo, TReturn> provider)
        {
            var context = new TimerPipeContext(timer);

            return new StartElement<TimerPipeContext, TimerInfo, TReturn>(context, timer, provider);
        }

        public static async Task CompleteWithAsync<TInput, TInputForNextStep>(
            this IPipeElement<TimerPipeContext, TInput, TInputForNextStep> element,
            ITimerFinalStepProvider<TInputForNextStep> provider)
        {
            var finalStep = new FinalElement<TimerPipeContext, TInputForNextStep>(
                element.Context, element.PreviousElements.Append(element), provider);

            await finalStep.ResolveAsync();
        }
    }
}
