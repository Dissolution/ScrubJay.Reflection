namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class MarkLabelInstruction : ILGeneratorInstruction
{
    public ILLabel Label { get; }

    public MarkLabelInstruction(ILLabel label)
        : base(ILGeneratorMethod.MarkLabel)
    {
        Label = label;
    }

    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder.Render(Label);
    }
}