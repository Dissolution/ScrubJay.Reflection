
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Unconditionally branches to the given label.
    /// </summary>
    public Emit Branch(SigilLabel sigilLabel)
    {
        _innerEmit.Branch(sigilLabel);
        return this;
    }

    /// <summary>
    /// Unconditionally branches to the label with the given name.
    /// </summary>
    public Emit Branch(string name)
    {
        _innerEmit.Branch(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, if both are equal branches to the given label.
    /// </summary>
    public Emit BranchIfEqual(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfEqual(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, if both are equal branches to the label with the given name.
    /// </summary>
    public Emit BranchIfEqual(string name)
    {
        _innerEmit.BranchIfEqual(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the given label.
    /// </summary>
    public Emit UnsignedBranchIfNotEqual(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfNotEqual(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the label with the given name.
    /// </summary>
    public Emit UnsignedBranchIfNotEqual(string name)
    {
        _innerEmit.UnsignedBranchIfNotEqual(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value.
    /// </summary>
    public Emit BranchIfGreaterOrEqual(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfGreaterOrEqual(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than or equal to the first value.
    /// </summary>
    public Emit BranchIfGreaterOrEqual(string name)
    {
        _innerEmit.BranchIfGreaterOrEqual(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value (when treated as unsigned values).
    /// </summary>
    public Emit UnsignedBranchIfGreaterOrEqual(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfGreaterOrEqual(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than or equal to the first value (when treated as unsigned values).
    /// </summary>
    public Emit UnsignedBranchIfGreaterOrEqual(string name)
    {
        _innerEmit.UnsignedBranchIfGreaterOrEqual(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value.
    /// </summary>
    public Emit BranchIfGreater(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfGreater(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than the first value.
    /// </summary>
    public Emit BranchIfGreater(string name)
    {
        _innerEmit.BranchIfGreater(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value (when treated as unsigned values).
    /// </summary>
    public Emit UnsignedBranchIfGreater(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfGreater(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than the first value (when treated as unsigned values).
    /// </summary>
    public Emit UnsignedBranchIfGreater(string name)
    {
        _innerEmit.UnsignedBranchIfGreater(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value.
    /// </summary>
    public Emit BranchIfLessOrEqual(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfLessOrEqual(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than or equal to the first value.
    /// </summary>
    public Emit BranchIfLessOrEqual(string name)
    {
        _innerEmit.BranchIfLessOrEqual(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value (when treated as unsigned values).
    /// </summary>
    public Emit UnsignedBranchIfLessOrEqual(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfLessOrEqual(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than or equal to the first value (when treated as unsigned values).
    /// </summary>
    public Emit UnsignedBranchIfLessOrEqual(string name)
    {
        _innerEmit.UnsignedBranchIfLessOrEqual(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value.
    /// </summary>
    public Emit BranchIfLess(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfLess(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than the first value.
    /// </summary>
    public Emit BranchIfLess(string name)
    {
        _innerEmit.BranchIfLess(name);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value (when treated as unsigned values).
    /// </summary>
    public Emit UnsignedBranchIfLess(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfLess(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than the first value (when treated as unsigned values).
    /// </summary>
    public Emit UnsignedBranchIfLess(string name)
    {
        _innerEmit.UnsignedBranchIfLess(name);
        return this;
    }

    /// <summary>
    /// Pops one argument from the stack, branches to the given label if the value is false.
    ///
    /// A value is false if it is zero or null.
    /// </summary>
    public Emit BranchIfFalse(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfFalse(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops one argument from the stack, branches to the label with the given name if the value is false.
    ///
    /// A value is false if it is zero or null.
    /// </summary>
    public Emit BranchIfFalse(string name)
    {
        _innerEmit.BranchIfFalse(name);
        return this;
    }

    /// <summary>
    /// Pops one argument from the stack, branches to the given label if the value is true.
    ///
    /// A value is true if it is non-zero or non-null.
    /// </summary>
    public Emit BranchIfTrue(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfTrue(sigilLabel);
        return this;
    }

    /// <summary>
    /// Pops one argument from the stack, branches to the label with the given name if the value is true.
    ///
    /// A value is true if it is non-zero or non-null.
    /// </summary>
    public Emit BranchIfTrue(string name)
    {
        _innerEmit.BranchIfTrue(name);
        return this;
    }
}
