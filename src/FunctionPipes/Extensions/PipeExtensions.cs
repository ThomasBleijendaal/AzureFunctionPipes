using System.Linq;
using FunctionPipes.Abstractions;
using FunctionPipes.Elements;
using Microsoft.AspNetCore.Http;

namespace FunctionPipes.Extensions
{
    // TODO: move these to namespaces
    public static class PipeExtensions
    {
        public static IPipeElement<TContext, TInputForNextStep, TReturn> DoNext<TContext, TInput, TInputForNextStep, TReturn>(
            this IPipeElement<TContext, TInput, TInputForNextStep> element,
            IStepProvider<TContext, TInputForNextStep, TReturn> provider)
        {
            return new Element<TContext, TInputForNextStep, TReturn>(
                element.Context, element.PreviousElements.Append(element), provider);
        }
    }
}
