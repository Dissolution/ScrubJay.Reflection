using System.Text.RegularExpressions;

namespace ScrubJay.Reflection.IL;

[PublicAPI]
public static class OpCoding
{
    private const byte IS_TWO_BYTE_OP_CODE = 0xFE;
    internal static readonly OpCode[] _oneByteOpCodes;
    internal static readonly OpCode[] _twoByteOpCodes;


    public static IReadOnlyList<OpCode> AllOpCodes { get; } = Shard<OpCodes>()
        .Fields()
        .Containing<OpCode>()
        .ToEnumerable()
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
            OpCode opCode = _oneByteOpCodes[op];
            if (opCode.Name is null)
                return new InvalidOperationException($"Byte '{op}' is not a valid OpCode byte");
            return Ok(opCode);
        }
        else
        {
            if (!reader.TryTake().IsSome(out op))
                return new InvalidOperationException("Could not read second OpCode byte");
            if (op >= 31)
                return new InvalidOperationException($"Byte '{op}' is not a valid second OpCode byte");
            OpCode opCode = _twoByteOpCodes[op];
            if (opCode == default)
                return new InvalidOperationException($"Byte '{op}' is not a valid second OpCode byte");
            return Ok(opCode);
        }
    }

    public static bool IsPrefix(this OpCode op) =>
        op == OpCodes.Unaligned ||
        op == OpCodes.Readonly ||
        op == OpCodes.Volatile ||
        op == OpCodes.Tailcall;

    public static bool TargetsLocalVariable(this OpCode opCode)
    {
        return TextHelper.Contains(opCode.Name, "loc", StringComparison.OrdinalIgnoreCase);
    }

    public static Option<Option<int>> TargetsLocal(this OpCode opCode)
    {
        var regex = new Regex(@"(?:ld|st)loc[\.as]*(\d)?", RegexOptions.Compiled);
        var match = regex.Match(opCode.Name);
        if (match.Success)
        {
            if (match.Groups.Count >= 2 && int.TryParse(match.Groups[1].Value, out int argIndex))
            {
                return Some(Some(argIndex));
            }
            else
            {
                return Some(None<int>());
            }
        }

        return None();
    }

    public static Option<Option<int>> TargetsArgument(this OpCode opCode)
    {
        var regex = new Regex(@"(?:ld|st)arg[\.as]*(\d)?", RegexOptions.Compiled);
        var match = regex.Match(opCode.Name);
        if (match.Success)
        {
            if (match.Groups.Count >= 2 && int.TryParse(match.Groups[1].Value, out int argIndex))
            {
                return Some(Some(argIndex));
            }
            else
            {
                return Some(None<int>());
            }
        }

        return None();
    }

    public static Option<int> TargetsI32Const(this OpCode opCode)
    {
        if (TextHelper.StartsWith(opCode.Name, "ldc.i4."))
        {
            if (TextHelper.EndsWith(opCode.Name, "m1"))
            {
                return Some(-1);
            }

            char ch = opCode.Name[^1];
            if (ch >= '0' && ch <= '8')
            {
                return Some(ch - '0');
            }
        }

        return None<int>();
    }
}