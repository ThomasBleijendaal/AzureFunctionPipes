namespace FunctionPipes.Abstractions.Providers
{
    public interface ISyncStepProvider : IStepProvider
    {
        internal object InternalDo(object context, object input);
    }

    public interface ISyncStepProvider<TContext, TInput, TReturn> : ISyncStepProvider
    {
        TReturn Do(TContext context, TInput input);

        object ISyncStepProvider.InternalDo(object context, object input)
        {
            return Do((TContext)context, (TInput)input)!;
        }
    }
}
