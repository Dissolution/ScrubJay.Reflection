namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class ILGeneratorInstruction : Instruction
{
    public ILGeneratorMethod ILGenMethod { get;  }

    public override sealed int Size => 0;
    
    protected ILGeneratorInstruction(ILGeneratorMethod ilGenMethod)
    {
        ILGenMethod = ilGenMethod;
    }
}