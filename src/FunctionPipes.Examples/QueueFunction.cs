using System;
using System.Threading.Tasks;
using FunctionPipes.Abstractions;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Extensions;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace FunctionPipes.Examples
{
    public class QueueFunction
    {
        private readonly IPipe _pipe;
        private readonly MessageDeserializer _messageDeserializer;
        private readonly MessageValidator _messageValidator;

        public QueueFunction(
            IPipe pipe,
            MessageDeserializer messageDeserializer,
            MessageValidator messageValidator)
        {
            _pipe = pipe;
            _messageDeserializer = messageDeserializer;
            _messageValidator = messageValidator;
        }

        [Disable]
        [FunctionName("QueueFunction")]
        public async Task Run([QueueTrigger("myqueue-items")]string myQueueItem)
        {
            await _pipe
                .StartWithQueueMessage(myQueueItem, _messageDeserializer)
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
