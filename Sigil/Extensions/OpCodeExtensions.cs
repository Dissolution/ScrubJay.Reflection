namespace ScrubJay.Sigil.Extensions;

/// <summary>
/// Extensions on <see cref="OpCode"/>
/// </summary>
[PublicAPI]
public static class OpCodeExtensions
{
    public static bool IsTailableCall(this OpCode op)
    {
        return
            op == OpCodes.Call ||
            //op == OpCodes.Calli ||
            op == OpCodes.Callvirt;
    }

    public static bool IsPrefix(this OpCode op)
    {
        return
            op == OpCodes.Tailcall ||
            op == OpCodes.Readonly ||
            op == OpCodes.Volatile ||
            op == OpCodes.Unaligned;
    }
}
