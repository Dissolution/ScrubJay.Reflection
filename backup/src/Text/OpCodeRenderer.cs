namespace ScrubJay.Reflection.Text;

[PublicAPI]
public sealed class OpCodeRenderer : Renderer<OpCode>
{
    public override void RenderTo(OpCode opCode, TextBuilder builder)
    {
        string name = opCode.Name.ThrowIfNull();
        builder.Append(name);
    }
}