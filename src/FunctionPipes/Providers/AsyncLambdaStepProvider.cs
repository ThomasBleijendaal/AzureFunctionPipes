using System;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Providers;

namespace FunctionPipes.Providers
{
    internal class AsyncLambdaStepProvider<TContext, TInput, TReturn> : IAsyncStepProvider<TContext, TInput, TReturn>
    {
        private readonly Func<TContext, TInput, Task<TReturn>> _lambda;

        public AsyncLambdaStepProvider(Func<TContext, TInput, Task<TReturn>> lambda)
        {
            _lambda = lambda;
        }

        public async Task<TReturn> DoAsync(TContext context, TInput input)
        {
            return await _lambda.Invoke(context, input);
        }
    }
}
