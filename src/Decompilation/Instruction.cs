#pragma warning disable CS0618 // Type or member is obsolete

using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Decompilation;

[PublicAPI]
public sealed record class Instruction(ILOffset Offset, OpCode OpCode, Option<object?> Operand)
    : IRenderable
{
    public int Size
    {
        get
        {
            int size = this.OpCode.Size;
            switch (OpCode.OperandType)
            {
                case OperandType.InlineSwitch:
                {
                    var targets = Operand
                        .SomeOrThrow()
                        .ThrowIfNot<Instruction[]>();
                    size += ((1 + targets.Length) * 4);
                    break;
                }
                case OperandType.InlineI8:
                case OperandType.InlineR:
                {
                    size += 8;
                    break;
                }
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                {
                    size += 4;
                    break;
                }
                case OperandType.InlineVar:
                {
                    size += 2;
                    break;
                }
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                {
                    size += 1;
                    break;
                }
                case OperandType.InlinePhi:
                case OperandType.InlineNone:
                {
                    break;
                }
                default:
                {
                    throw InvalidEnumException.New(OpCode.OperandType);
                }
            }

            return size;
        }
    }
    
    public TextBuilder RenderTo(TextBuilder builder) => builder
        .Render(Offset)
        .Append(": ")
        .Align(OpCode.Name, 14, alignment: Alignment.Right)
        .If(Operand, static (tb, op) => tb.Append("  ").Render(op));
}