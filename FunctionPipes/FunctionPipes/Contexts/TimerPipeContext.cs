using Microsoft.Azure.WebJobs;

namespace FunctionPipes.Contexts
{
    public sealed class TimerPipeContext : PipeContext
    {
        public TimerPipeContext(TimerInfo timerInfo)
        {
            TimerInfo = timerInfo;
        }

        public TimerInfo TimerInfo { get; }
    }
}
