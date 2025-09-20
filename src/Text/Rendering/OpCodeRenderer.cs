using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class OpCodeRenderer : Renderer<OpCode>
{
    public override TextBuilder RenderTo(TextBuilder builder, OpCode opCode)
    {
        string name = opCode.Name.ThrowIfNull();
        return builder.Append(name);
    }
}