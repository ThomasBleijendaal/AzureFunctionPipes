using Microsoft.AspNetCore.Http;

namespace FunctionPipes.Contexts
{
    public sealed class HttpPipeContext : PipeContext
    {
        public HttpPipeContext(HttpRequest request)
        {
            Request = request;
        }

        public HttpRequest Request { get; }
    }
}
