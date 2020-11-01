using FunctionPipes;
using FunctionPipes.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFunctionPipes(this IServiceCollection services)
        {
            services.AddTransient<IPipe, PipeStart>();

            return services;
        }
    }
}
