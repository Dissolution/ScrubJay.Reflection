namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class BeginExceptionBlockInstruction : ILGeneratorInstruction
{
    public ILLabel Label { get; }

    public BeginExceptionBlockInstruction(ILLabel label)
        : base(ILGeneratorMethod.BeginExceptionBlock)
    {
        Label = label;
    }

    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder.Render(Label);
    }
}