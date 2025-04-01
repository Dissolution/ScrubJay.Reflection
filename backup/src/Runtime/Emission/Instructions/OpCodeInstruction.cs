using ScrubJay.Reflection.Naming;

namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public sealed record class OpCodeInstruction : Instruction
{
    public required OpCode OpCode { get; init; }

    public Option<object> Operand { get; internal set; }

    public override int Size
    {
        get
        {
            int size = OpCode.Size;

            switch (OpCode.OperandType)
            {
                case OperandType.InlineSwitch:
                {
                    if (!Operand.IsSome(out var op))
                        throw new InvalidOperationException();

                    if (op is int[] offsets)
                    {
                        size += (1 + offsets.Length) * 4;
                    }
                    else if (op is Instruction[] instructions)
                    {
                        size += (1 + instructions.Length) * 4;
                    }
                    else
                    {
                        Debugger.Break();
                        throw new InvalidOperationException();
                    }
                    break;
                }
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    size += 8;
                    break;
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    size += 4;
                    break;
                case OperandType.InlineVar:
                    size += 2;
                    break;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    size += 1;
                    break;
                //case OperandType.InlinePhi:
                case OperandType.InlineNone:
                default:
                    break; // no change
            }

            return size;
        }
    }

    public OpCodeInstruction() { }

    [SetsRequiredMembers]
    public OpCodeInstruction(OpCode opCode)
    {
        OpCode = opCode;
        Operand = None();
    }

    /*[SetsRequiredMembers]
    public OpCodeInstruction(OpCode opCode, object operand)
    {
        OpCode = opCode;
        Operand = Some(operand.ThrowIfNull());
    }*/


    private TextBuilder AppendTo(TextBuilder text)
    {
        var alignedName = $"{OpCode.Name,-14}";
        text.Append(alignedName);
        if (!Operand.IsSome(out var operand))
            return text;

        Debug.Assert(operand is not null);
        text.Append(' ');

        switch (operand)
        {
            case sbyte i8:
                return text.Append("(sbyte)").Append(i8);
            case byte u8:
                return text.Append("(byte)").Append(u8);
            case short i16:
                return text.Append("(short)").Append(i16);
            case ushort u16:
                return text.Append("(ushort)").Append(u16);
            case int i32:
                return text.Append(i32);
            case uint u32:
                return text.Append(u32).Append('U');
            case long i64:
                return text.Append(i64).Append('L');
            case ulong u64:
                return text.Append(u64).Append("UL");
            case float f32:
                return text.Append(f32, "G").Append('f');
            case double f64:
                return text.Append(f64, "G").Append('d');
            case string str:
                return text.Append('\"').Append(str).Append('\"');
            case MemberInfo memberInfo:
                return text.Append('`').AppendMember(memberInfo).Append('`');
            case ParameterInfo parameterInfo:
                return text.Append('`').AppendParameter(parameterInfo).Append('`');
            case Instruction instruction:
            {
                return text.Append('|')
                    .Invoke(tb => instruction.AppendOffset(tb))
                    .Append('|');
            }
            case LocalVariableInfo localVariable:
            {
                return text
                    .Append('`')
                    .Append(new EmitterLocal(localVariable))
                    .Append('`');
            }
            default:
            {
                var type = operand?.GetType();
                throw new NotImplementedException();
            }
        }
    }

    public override string ToString()
    {
        return TextBuilder.New
            .Invoke(AppendOffset)
            .Append(": ")
            .Invoke(AppendTo)
            .ToStringAndDispose();
    }
}
