using System;

namespace FunctionPipes.Contexts
{
    public sealed class QueuePipeContext : PipeContext
    {
        public QueuePipeContext(
            IServiceProvider serviceProvider, 
            string message) : base(serviceProvider)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
