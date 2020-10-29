using System;
using Microsoft.AspNetCore.Http;

namespace FunctionPipes.Contexts
{
    public sealed class HttpPipeContext : PipeContext
    {
        public HttpPipeContext(
            IServiceProvider serviceProvider, 
            HttpRequest request) : base(serviceProvider)
        {
            Request = request;
        }

        public HttpRequest Request { get; }
    }
}
