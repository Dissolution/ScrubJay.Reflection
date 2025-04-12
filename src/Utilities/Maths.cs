using InlineIL;
using static InlineIL.IL;

namespace ScrubJay.Reflection.Utilities;

public static class Maths
{
    public static int FibonacciTailRecursive(int n, int previous = 0, int current = 1)
    {
        if (n == 0)
            return previous;
        if (n == 1)
            return current;
    
        return FibonacciTailRecursive(n - 1, current, previous + current);
    }

    public static int FibonacciTailCall(int n, int previous = 0, int current = 1)
    {
        if (n == 0)
            return previous;
        if (n == 1)
            return current;

        Emit.Ldarg(nameof(n));
        Emit.Ldc_I4_1();
        Emit.Sub();
        Emit.Ldarg(nameof(current));
        Emit.Ldarg(nameof(current));
        Emit.Ldarg(nameof(previous));
        Emit.Add();
        Emit.Tail();
        Emit.Call(MethodRef.Method(typeof(Maths), nameof(FibonacciTailCall)));
        Emit.Ret();
        throw Unreachable();
    }
}