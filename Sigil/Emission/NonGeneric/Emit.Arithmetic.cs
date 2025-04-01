

namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops two arguments off the stack, adds them, and pushes the result.
    /// </summary>
    public Emit Add()
    {
        _innerEmit.Add();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, adds them, and pushes the result.
    /// 
    /// Throws an OverflowException if the result overflows the destination type.
    /// </summary>
    public Emit AddOverflow()
    {
        _innerEmit.AddOverflow();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, adds them as if they were unsigned, and pushes the result.
    /// 
    /// Throws an OverflowException if the result overflows the destination type.
    /// </summary>
    public Emit UnsignedAddOverflow()
    {
        _innerEmit.UnsignedAddOverflow();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, divides the second by the first, and pushes the result.
    /// </summary>
    public Emit Divide()
    {
        _innerEmit.Divide();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, divides the second by the first as if they were unsigned, and pushes the result.
    /// </summary>
    public Emit UnsignedDivide()
    {
        _innerEmit.UnsignedDivide();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, multiplies them, and pushes the result.
    /// </summary>
    public Emit Multiply()
    {
        _innerEmit.Multiply();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, multiplies them, and pushes the result.
    /// 
    /// Throws an OverflowException if the result overflows the destination type.
    /// </summary>
    public Emit MultiplyOverflow()
    {
        _innerEmit.MultiplyOverflow();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, multiplies them as if they were unsigned, and pushes the result.
    /// 
    /// Throws an OverflowException if the result overflows the destination type.
    /// </summary>
    public Emit UnsignedMultiplyOverflow()
    {
        _innerEmit.UnsignedMultiplyOverflow();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, calculates the remainder of the second divided by the first, and pushes the result.
    /// </summary>
    public Emit Remainder()
    {
        _innerEmit.Remainder();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, calculates the remainder of the second divided by the first as if both were unsigned, and pushes the result.
    /// </summary>
    public Emit UnsignedRemainder()
    {
        _innerEmit.UnsignedRemainder();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, subtracts the first from the second, and pushes the result.
    /// </summary>
    public Emit Subtract()
    {
        _innerEmit.Subtract();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, subtracts the first from the second, and pushes the result.
    /// 
    /// Throws an OverflowException if the result overflows the destination type.
    /// </summary>
    public Emit SubtractOverflow()
    {
        _innerEmit.SubtractOverflow();
        return this;
    }

    /// <summary>
    /// Pops two arguments off the stack, subtracts the first from the second as if they were unsigned, and pushes the result.
    /// 
    /// Throws an OverflowException if the result overflows the destination type.
    /// </summary>
    public Emit UnsignedSubtractOverflow()
    {
        _innerEmit.UnsignedSubtractOverflow();
        return this;
    }

    /// <summary>
    /// Pops an argument off the stack, negates it, and pushes the result.
    /// </summary>
    public Emit Negate()
    {
        _innerEmit.Negate();
        return this;
    }
}