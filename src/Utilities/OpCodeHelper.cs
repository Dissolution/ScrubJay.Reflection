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
                yield return code;
            foreach (var code in TwoByteOpCodes)
                yield return code;
        }
    }

    static OpCodeHelper()
    {
        OneByteOpCodes = new OpCode[0xE1];
        TwoByteOpCodes = new OpCode[0x1F];

        var opCodeFields = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in opCodeFields)
        {
            OpCode opCode = (OpCode)field.GetValue(null);
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
}