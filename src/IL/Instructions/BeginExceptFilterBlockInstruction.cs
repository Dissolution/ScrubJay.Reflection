namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class BeginExceptFilterBlockInstruction : ILGeneratorInstruction
{
    public BeginExceptFilterBlockInstruction() 
        : base(ILGeneratorMethod.BeginExceptFilterBlock)
    {
        
    }
}