using System;
using System.Threading.Tasks;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using FunctionPipes.Extensions.Timer;
using Microsoft.Azure.WebJobs;

namespace FunctionPipes.Examples
{
    public class TimerFunction
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LogTime _logTime;
        private readonly WaitSomeTime _waitSomeTime;

        public TimerFunction(
            IServiceProvider serviceProvider,
            LogTime logTime,
            WaitSomeTime waitSomeTime)
        {
            _serviceProvider = serviceProvider;
            _logTime = logTime;
            _waitSomeTime = waitSomeTime;
        }

        [FunctionName("TimerFunction")]
        public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)]TimerInfo myTimer)
        {
            await myTimer
                .StartWith(_serviceProvider, _logTime)
                .CompleteWithAsync(_waitSomeTime);
        }

        public class TimerData
        {
            public DateTime Now { get; set; }
        }

        public class LogTime : ITimerStepProvider<TimerInfo, TimerData>
        {
            public async Task<TimerData> DoAsync(TimerPipeContext context, TimerInfo input)
            {
                var now = DateTime.UtcNow;

                await Task.Delay(5000);

                return new TimerData { Now = now };
            }
        }

        public class WaitSomeTime : ITimerFinalStepProvider<TimerData>
        {
            public async Task<bool> FinalizeAsync(TimerPipeContext context, TimerData? input)
            {
                await Task.Delay(5000);

                return true;
            }
        }
    }
}
