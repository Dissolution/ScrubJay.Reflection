using ScrubJay.Memory;
using ScrubJay.Reflection.Exceptions;

namespace ScrubJay.Reflection.Utilities;

[PublicAPI]
public static class OpCodeHelper
{
    public static OpCode[] OneByteOpCodes { get; }
    public static OpCode[] TwoByteOpCodes { get; }

    public static IEnumerable<OpCode> OpCodes
    {
        get
        {
            foreach (var code in OneByteOpCodes)
            {
                if (code.Name is not null)
                    yield return code;
            }

            foreach (var code in TwoByteOpCodes)
            {
                if (code.Name is not null)
                    yield return code;
            }
        }
    }

    static OpCodeHelper()
    {
        OneByteOpCodes = new OpCode[0xE1];
        TwoByteOpCodes = new OpCode[0x1F];

        var opCodeFields = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        foreach (var field in opCodeFields)
        {
            OpCode opCode = (OpCode)field.GetValue(null)!;
            if (opCode.OpCodeType == OpCodeType.Nternal)
                continue;

            if (opCode.Size == 1)
            {
                OneByteOpCodes[opCode.Value] = opCode;
            }
            else
            {
                Debug.Assert(opCode.Size == 2);
                TwoByteOpCodes[opCode.Value & 0xFF] = opCode;
            }
        }
    }
    
    public static OpCode ReadOpCode(ref SpanReader<byte> reader)
    {
        OpCode opCode;

        byte u8 = reader.Take();

        if (u8 != 0xFE)
        {
            opCode = OneByteOpCodes[u8];
            if (string.IsNullOrEmpty(opCode.Name))
                throw new ReflectionException($"Invalid one-byte OpCode for 0x{u8:X}");
        }
        else
        {
            u8 = reader.Take();
            opCode = TwoByteOpCodes[u8];
            if (string.IsNullOrEmpty(opCode.Name))
                throw new ReflectionException($"Invalid two-byte OpCode for 0x{u8:X}");
        }

        return opCode;
    }

    public static Result<OpCode> TryReadOpCode(ref SpanReader<byte> reader)
    {
        OpCode opCode;
        byte u8 = reader.Take();

        if (u8 != 0xFE)
        {
            opCode = OneByteOpCodes[u8];
            if (string.IsNullOrEmpty(opCode.Name))
                return new ReflectionException($"Invalid one-byte OpCode for 0x{u8:X}");
        }
        else
        {
            u8 = reader.Take();
            opCode = TwoByteOpCodes[u8];
            if (string.IsNullOrEmpty(opCode.Name))
                return new ReflectionException($"Invalid two-byte OpCode for 0x{u8:X}");
        }

        return Ok(opCode);
    }
}