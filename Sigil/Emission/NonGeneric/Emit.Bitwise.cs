

namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops two arguments off the stack, performs a bitwise and, and pushes the result.
    /// </summary>
    public Emit And()
    {
        _innerEmit.And();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, performs a bitwise or, and pushes the result.
    /// </summary>
    public Emit Or()
    {
        _innerEmit.Or();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, performs a bitwise xor, and pushes the result.
    /// </summary>
    public Emit Xor()
    {
        _innerEmit.Xor();
        return this;
    }

    /// <summary>
    /// Pops one argument off the stack, performs a bitwise inversion, and pushes the result.
    /// </summary>
    public Emit Not()
    {
        _innerEmit.Not();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, shifts the second value left by the first value.
    /// </summary>
    public Emit ShiftLeft()
    {
        _innerEmit.ShiftLeft();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, shifts the second value right by the first value.
    /// 
    /// Sign extends from the left.
    /// </summary>
    public Emit ShiftRight()
    {
        _innerEmit.ShiftRight();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, shifts the second value right by the first value.
    /// 
    /// Acts as if the value were unsigned, zeros always coming in from the left.
    /// </summary>
    public Emit UnsignedShiftRight()
    {
        _innerEmit.UnsignedShiftRight();
        return this;
    }
}