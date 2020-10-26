namespace FunctionPipes.Contexts
{
    public sealed class QueuePipeContext : PipeContext
    {
        public QueuePipeContext(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
