using System.Text.RegularExpressions;

namespace ScrubJay.Reflection.IL;

[PublicAPI]
public static class OpCoding
{
    private const byte IS_TWO_BYTE_OP_CODE = 0xFE;
    internal static readonly OpCode[] _oneByteOpCodes;
    internal static readonly OpCode[] _twoByteOpCodes;


    public static IReadOnlyList<OpCode> AllOpCodes { get; } = Reflect<OpCodes>()
        .Fields.Returning<OpCode>()
        .Select(field => field.GetValue(null).ThrowIfNot<OpCode>())
        .ToList();

    static OpCoding()
    {
        _oneByteOpCodes = new OpCode[256];
        _twoByteOpCodes = new OpCode[31];

        foreach (var opCode in AllOpCodes)
        {
            ushort value = unchecked((ushort)opCode.Value);
            if (value < 0b_1_00000000)
            {
                Debug.Assert(opCode.Size == 1);
                _oneByteOpCodes[value] = opCode;
            }
            else // if ((value & 0b_11111111_00000000) == 0b_11111110_00000000)
            {
                Debug.Assert(opCode.Size == 2);
                Debug.Assert((value & 0xFF) < 31);
                _twoByteOpCodes[value & 0b_00000000_11111111] = opCode;
            }
        }
    }

    public static OpCode ReadOpCode(this ref SpanReader<byte> reader)
        => TryReadOpCode(ref reader).OkOrThrow();

    public static Result<OpCode> TryReadOpCode(this ref SpanReader<byte> reader)
    {
        if (!reader.TryTake().IsSome(out byte op))
            return new InvalidOperationException("Could not read OpCode byte");
        if (op != IS_TWO_BYTE_OP_CODE)
        {
            OpCode opcode = _oneByteOpCodes[op];
            if (opcode == default)
                return new InvalidOperationException($"Byte '{op}' is not a valid OpCode byte");
            return Ok(opcode);
        }
        else
        {
            if (!reader.TryTake().IsSome(out op))
                return new InvalidOperationException("Could not read second OpCode byte");
            if (op >= 31)
                return new InvalidOperationException($"Byte '{op}' is not a valid second OpCode byte");
            OpCode opcode = _twoByteOpCodes[op];
            if (opcode == default)
                return new InvalidOperationException($"Byte '{op}' is not a valid second OpCode byte");
            return Ok(opcode);
        }
    }

    public static bool IsPrefix(this OpCode op) =>
        op == OpCodes.Unaligned ||
        op == OpCodes.Readonly ||
        op == OpCodes.Volatile ||
        op == OpCodes.Tailcall;

    public static bool TargetsLocalVariable(this OpCode opcode)
    {
        return TextHelper.Contains(opcode.Name, "loc", StringComparison.OrdinalIgnoreCase);
    }

    public static bool TargetsLocalVariable(this OpCode opcode, out Option<int> index)
    {
        var regex = new Regex(@"(?:ld|st)loc[\.as]*(\d)?", RegexOptions.Compiled);
        var match = regex.Match(opcode.Name);
        if (match.Success)
        {
            var groups = match.Groups;

            if (groups.Count >= 2)
            {
                string integer = groups[1].Value;
                if (int.TryParse(integer, out int i))
                {
                    index = Some(i);
                    return true;
                }
                else
                {
                    index = None();
                    return true;
                }
            }

            Debugger.Break();
            throw new NotImplementedException();
        }
        else
        {
            index = None();
            return false;
        }
    }
    
    public static bool TargetsArgument(this OpCode opcode, out Option<int> index)
    {
        var regex = new Regex(@"(?:ld|st)arg[\.as]*(\d)?", RegexOptions.Compiled);
        var match = regex.Match(opcode.Name);
        if (match.Success)
        {
            var groups = match.Groups;

            if (groups.Count >= 2)
            {
                string integer = groups[1].Value;
                if (int.TryParse(integer, out int i))
                {
                    index = Some(i);
                    return true;
                }
                else
                {
                    index = None();
                    return true;
                }
            }

            Debugger.Break();
            throw new NotImplementedException();
        }
        else
        {
            index = None();
            return false;
        }
    }
}