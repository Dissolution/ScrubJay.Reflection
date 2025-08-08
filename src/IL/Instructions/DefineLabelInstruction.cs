namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class DefineLabelInstruction : ILGeneratorInstruction
{
    public ILLabel Label { get; }

    public DefineLabelInstruction(ILLabel label)
        : base(ILGeneratorMethod.DefineLabel)
    {
        Label = label;
    }

    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder.Render(Label);
    }
}