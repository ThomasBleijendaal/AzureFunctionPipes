using System;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Extensions.Queue;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace FunctionPipes.Examples
{
    public class QueueFunction
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageDeserializer _messageDeserializer;
        private readonly MessageValidator _messageValidator;

        public QueueFunction(
            IServiceProvider serviceProvider,
            MessageDeserializer messageDeserializer,
            MessageValidator messageValidator)
        {
            _serviceProvider = serviceProvider;
            _messageDeserializer = messageDeserializer;
            _messageValidator = messageValidator;
        }

        [FunctionName("QueueFunction")]
        public async Task Run([QueueTrigger("myqueue-items")]string myQueueItem)
        {
            await myQueueItem
                .StartWith(_serviceProvider, _messageDeserializer)
                .CompleteWithAsync(_messageValidator);
        }

        public class QueueMessage
        {
            public string Message { get; set; } = default!;
        }

        public class MessageDeserializer : IQueueStepProvider<string, QueueMessage>
        {
            public Task<QueueMessage> DoAsync(QueuePipeContext context, string input)
            {
                return Task.FromResult(JsonConvert.DeserializeObject<QueueMessage>(input) ?? throw new FormatException());
            }
        }

        public class MessageValidator : IQueueFinalStepProvider<QueueMessage>
        {
            public Task<bool> FinalizeAsync(QueuePipeContext context, QueueMessage? input)
            {
                if (context.ThrownException != null)
                {
                    throw context.ThrownException;
                }

                if (input == null)
                {
                    throw new InvalidOperationException();
                }

                if (input.Message == "Hi")
                {
                    throw new Exception();
                }

                return Task.FromResult(true);
            }
        }
    }
}
