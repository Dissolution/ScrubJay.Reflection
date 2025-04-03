using System.Reflection.Emit;

namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions on <see cref="OpCode"/>
/// </summary>
[PublicAPI]
public static class OpCodeExtensions
{
    public static bool IsPrefix(this OpCode op) => 
        op == OpCodes.Unaligned ||
        op == OpCodes.Readonly ||
        op == OpCodes.Volatile ||
        op == OpCodes.Tailcall;
}
