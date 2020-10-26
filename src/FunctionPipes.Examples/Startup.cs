using FunctionPipes;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using static FunctionPipes.Examples.HttpFunction;
using static FunctionPipes.Examples.QueueFunction;
using static FunctionPipes.Examples.TimerFunction;

[assembly: FunctionsStartup(typeof(Startup))]
namespace FunctionPipes
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // http
            builder.Services.AddSingleton<Authenicate>();
            builder.Services.AddSingleton<SerializeBodyTo<ExampleBody>>();
            builder.Services.AddSingleton<CallToExternalService<ExampleBody, ExampleResult>>();
            builder.Services.AddSingleton<ReturnResult>();

            // timer
            builder.Services.AddSingleton<LogTime>();
            builder.Services.AddSingleton<WaitSomeTime>();

            // queue
            builder.Services.AddSingleton<MessageDeserializer>();
            builder.Services.AddSingleton<MessageValidator>();
        }
    }
}
