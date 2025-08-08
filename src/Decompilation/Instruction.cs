using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Decompilation;

[PublicAPI]
public sealed record class Instruction(ILOffset Offset, OpCode OpCode, Option<object?> Operand)
    : IRenderable
{
    public void RenderTo(TextBuilder builder) => builder
        .Render(Offset)
        .Append(": ")
        .Align(OpCode.Name, 13, alignment: Alignment.Right)
        .If(Operand, static (tb, op) => tb.Append("  ").Render(op));
}