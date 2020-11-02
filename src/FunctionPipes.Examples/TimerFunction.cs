using System;
using System.Threading.Tasks;
using FunctionPipes.Abstractions;
using FunctionPipes.Abstractions.Providers;
using FunctionPipes.Contexts;
using Microsoft.Azure.WebJobs;

namespace FunctionPipes.Examples
{
    public class TimerFunction
    {
        private readonly IPipe _pipe;
        private readonly AsyncLogTime _asyncLogTime;
        private readonly SyncLogTime _syncLogTime;
        private readonly WaitSomeTime _waitSomeTime;

        public TimerFunction(
            IPipe pipe,
            AsyncLogTime asyncLogTime,
            SyncLogTime syncLogTime,
            WaitSomeTime waitSomeTime)
        {
            _pipe = pipe;
            _asyncLogTime = asyncLogTime;
            _syncLogTime = syncLogTime;
            _waitSomeTime = waitSomeTime;
        }

        [Disable]
        [FunctionName("TimerFunction")]
        public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)]TimerInfo myTimer)
        {
            await _pipe
                .StartWithTimer(myTimer, _syncLogTime)
                .DoNext(_asyncLogTime)
                .DoNext((context, input) => new TimerData { Now = DateTime.UtcNow })
                .CompleteWithAsync(_waitSomeTime);
        }

        public class TimerData
        {
            public DateTime Now { get; set; }
        }

        public class AsyncLogTime : IAsyncTimerStepProvider<TimerInfo, TimerInfo>
        {
            public async Task<TimerInfo> DoAsync(TimerPipeContext context, TimerInfo input)
            {
                var now = DateTime.UtcNow;

                await Task.Delay(1000);

                return input;
            }
        }

        public class SyncLogTime : ISyncTimerStepProvider<TimerInfo, TimerInfo>
        {
            public TimerInfo Do(TimerPipeContext context, TimerInfo input)
            {
                var now = DateTime.UtcNow;

                return input;
            }
        }

        public class WaitSomeTime : IAsyncTimerStepProvider<TimerData?, bool>
        {
            public async Task<bool> DoAsync(TimerPipeContext context, TimerData? input)
            {
                await Task.Delay(5000);

                return true;
            }
        }
    }
}
