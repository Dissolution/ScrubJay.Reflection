
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Pops two values from the stack, and pushes a 1 if they are equal and 0 if they are not.
    /// 
    /// New value on the stack is an Int32.
    /// </summary>
    public Emit CompareEqual()
    {
        _innerEmit.CompareEqual();
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, pushes a 1 if the second value is greater than the first value and a 0 otherwise.
    /// 
    /// New value on the stack is an Int32.
    /// </summary>
    public Emit CompareGreaterThan()
    {
        _innerEmit.CompareGreaterThan();
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, pushes a 1 if the second value is greater than the first value (as unsigned values) and a 0 otherwise.
    /// 
    /// New value on the stack is an Int32.
    /// </summary>
    public Emit UnsignedCompareGreaterThan()
    {
        _innerEmit.UnsignedCompareGreaterThan();
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, pushes a 1 if the second value is less than the first value and a 0 otherwise.
    /// 
    /// New value on the stack is an Int32.
    /// </summary>
    public Emit CompareLessThan()
    {
        _innerEmit.CompareLessThan();
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, pushes a 1 if the second value is less than the first value (as unsigned values) and a 0 otherwise.
    /// 
    /// New value on the stack is an Int32.
    /// </summary>
    public Emit UnsignedCompareLessThan()
    {
        _innerEmit.UnsignedCompareLessThan();
        return this;
    }
}