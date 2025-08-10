namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class DeclareLocalInstruction : ILGeneratorInstruction
{
    public ILLocal Local { get; }

    public DeclareLocalInstruction(ILLocal local)
        : base(ILGeneratorMethod.DeclareLocal)
    {
        Local = local;
    }
    
    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder.Render(Local);
    }
}