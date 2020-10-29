using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FunctionPipes.Contexts
{

    public class PipeContext
    {
        public PipeContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            Logger = serviceProvider.GetRequiredService<ILogger>();
        }

        public Exception? ThrownException { get; set; }

        public ILogger Logger { get; }

        public void CachePipeOutput(object key, TimeSpan? lifeTime = null)
        {
            EnableCache = true;
            CacheFromThisStep = true;
            CacheKey = key ?? throw new ArgumentNullException(nameof(key));
            CacheLifeTime = lifeTime;
        }

        internal bool EnableCache { get; set; }
        internal bool CacheFromThisStep { get; set; }
        internal object? CacheKey { get; set; }
        internal TimeSpan? CacheLifeTime { get; set; }

        internal IServiceProvider ServiceProvider { get; }
    }
}
