namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class EndExceptionBlockInstruction : ILGeneratorInstruction
{
    public EndExceptionBlockInstruction() 
        : base(ILGeneratorMethod.EndExceptionBlock)
    {
        
    }
}

[PublicAPI]
public sealed class BeginFaultBlockInstruction : ILGeneratorInstruction
{
    public BeginFaultBlockInstruction() 
        : base(ILGeneratorMethod.BeginFaultBlock)
    {
        
    }
}

[PublicAPI]
public sealed class BeginFinallyBlockInstruction : ILGeneratorInstruction
{
    public BeginFinallyBlockInstruction() 
        : base(ILGeneratorMethod.BeginFinallyBlock)
    {
        
    }
}

[PublicAPI]
public sealed class BeginScopeInstruction : ILGeneratorInstruction
{
    public BeginScopeInstruction() 
        : base(ILGeneratorMethod.BeginScope)
    {
        
    }
}

[PublicAPI]
public sealed class EndScopeInstruction : ILGeneratorInstruction
{
    public EndScopeInstruction() 
        : base(ILGeneratorMethod.EndScope)
    {
        
    }
}