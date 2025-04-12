using ScrubJay.Reflection.IL.Instructions;

namespace ScrubJay.Reflection.IL.Emission;

public interface IEmitter<S>
    where S : IEmitter<S>
{
    InstructionStream Instructions { get; }
}

public class GenEmitter : IEmitter<GenEmitter>
{
    protected readonly ILGenerator? _generator;

    public InstructionStream Instructions { get; } = [];

    public GenEmitter(ILGenerator? generator)
    {
        _generator = generator;
        
    }
}   