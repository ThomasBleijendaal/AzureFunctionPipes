using System;
using FunctionPipes.Abstractions.Providers;

namespace FunctionPipes.Providers
{
    internal class SyncLambdaStepProvider<TContext, TInput, TReturn> : ISyncStepProvider<TContext, TInput, TReturn>
    {
        private readonly Func<TContext, TInput, TReturn> _lambda;

        public SyncLambdaStepProvider(Func<TContext, TInput, TReturn> lambda)
        {
            _lambda = lambda;
        }

        public TReturn Do(TContext context, TInput input)
        {
            return _lambda.Invoke(context, input);
        }
    }
}
