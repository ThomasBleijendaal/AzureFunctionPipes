using System;
using Microsoft.Azure.WebJobs;

namespace FunctionPipes.Contexts
{
    public sealed class TimerPipeContext : PipeContext
    {
        public TimerPipeContext(
            IServiceProvider serviceProvider, 
            TimerInfo timerInfo) : base(serviceProvider)
        {
            TimerInfo = timerInfo;
        }

        public TimerInfo TimerInfo { get; }
    }
}
