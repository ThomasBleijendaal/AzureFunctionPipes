using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions
{
    public interface IStartElement
    {
        object Input { get; }
        PipeContext Context { get; }
    }
}
