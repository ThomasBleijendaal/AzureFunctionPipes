using FunctionPipes.Contexts;

namespace FunctionPipes.Abstractions.Elements
{
    public interface IStartElement
    {
        object Input { get; }
        PipeContext Context { get; }
    }
}
