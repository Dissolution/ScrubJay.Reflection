namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class BeginCatchBlockInstruction : ILGeneratorInstruction
{
    public Type? ExceptionType { get; }
    
    public BeginCatchBlockInstruction(Type? exceptionType) 
        : base(ILGeneratorMethod.BeginCatchBlock)
    {
        this.ExceptionType = exceptionType;
    }

    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder.Render(ExceptionType);
    }
}