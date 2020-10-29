using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Extensions.Activity;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionPipes.Examples
{
    public class DurableFunction
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ActivityLogger _activityLogger;
        private readonly DoSomething _doSomething;

        public DurableFunction(
            IServiceProvider serviceProvider,
            ActivityLogger activityLogger,
            DoSomething doSomething)
        {
            _serviceProvider = serviceProvider;
            _activityLogger = activityLogger;
            _doSomething = doSomething;
        }

        [FunctionName("DurableFunction")]
        public async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>
            {

                // Replace "hello" with the name of your Durable Activity Function.
                await context.CallActivityAsync<string>("DurableFunction_Hello", new ActivityModel { Name = "Tokyo" }),
                await context.CallActivityAsync<string>("DurableFunction_Hello", new ActivityModel { Name = "Seattle" }),
                await context.CallActivityAsync<string>("DurableFunction_Hello", new ActivityModel { Name = "London" })
            };

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("DurableFunction_Hello")]
        public async Task<string> SayHello([ActivityTrigger] ActivityModel request, ILogger log)
        {
            return await request
                .StartWith(_serviceProvider, _activityLogger)
                .CompleteWithAsync(_doSomething);
        }

        [FunctionName("DurableFunction_TimerStart")]
        public async Task TimerStart(
            [TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo timer,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var instanceId = await starter.StartNewAsync("DurableFunction", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        }

        public class ActivityModel
        {
            public string Name { get; set; } = default!;
        }

        public class ActivityLogger : IActivityStepProvider<ActivityModel, string>
        {
            public Task<string> DoAsync(PipeContext context, ActivityModel input)
            {
                Console.WriteLine(input.Name);

                return Task.FromResult(input.Name);
            }
        }

        public class DoSomething : IActivityFinalStepProvider<string, string>
        {
            public Task<string> FinalizeAsync(PipeContext context, string? input)
            {
                if (context.ThrownException != null)
                {
                    Console.WriteLine(context.ThrownException.Message);
                }

                if (input == null)
                {
                    throw new Exception();
                }

                Console.WriteLine(input);

                return Task.FromResult(input);
            }
        }
    }
}
