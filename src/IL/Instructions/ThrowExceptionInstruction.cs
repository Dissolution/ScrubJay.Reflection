namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class ThrowExceptionInstruction : ILGeneratorInstruction
{
    public Type ExceptionType { get; }
    
    public ThrowExceptionInstruction(Type exceptionType) 
        : base(ILGeneratorMethod.ThrowException)
    {
        this.ExceptionType = exceptionType;
        // todo: verify new()!
    }

    protected internal override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder.Render(ExceptionType);
    }
}