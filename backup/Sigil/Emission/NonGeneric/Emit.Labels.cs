
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Defines a new label.
    ///
    /// This label can be used for branching, leave, and switch instructions.
    ///
    /// A label must be marked exactly once after being defined, using the MarkLabel() method.
    /// </summary>
    public SigilLabel DefineLabel(string name = null)
    {
        return _innerEmit.DefineLabel(name);
    }

    /// <summary>
    /// Defines a new label.
    ///
    /// This label can be used for branching, leave, and switch instructions.
    ///
    /// A label must be marked exactly once after being defined, using the MarkLabel() method.
    /// </summary>
    public Emit DefineLabel(out SigilLabel sigilLabel, string name = null)
    {
        _innerEmit.DefineLabel(out sigilLabel, name);

        return this;
    }

    /// <summary>
    /// Marks a label in the instruction stream.
    ///
    /// When branching, leaving, or switching with a label control will be transfered to where it was *marked* not defined.
    ///
    /// Labels can only be marked once, and *must* be marked before creating a delegate.
    ///
    /// Logically after a Branch or Leave instruction, a stack assertion is required to continue emiting.  The stack
    /// is assumed to match that state in these cases.
    ///
    /// In all other cases, a stack assertion is merely checked (and if failing, a verification exception is thrown).
    ///
    /// In the assertion, the top of the stack is the first (0-indexed, left-most) parameter.
    /// </summary>
    public Emit MarkLabel(SigilLabel sigilLabel)
    {
        _innerEmit.MarkLabel(sigilLabel);
        return this;
    }

    /// <summary>
    /// Marks a label with the given name in the instruction stream.
    ///
    /// When branching, leaving, or switching with a label control will be transfered to where it was *marked* not defined.
    ///
    /// Labels can only be marked once, and *must* be marked before creating a delegate.
    /// </summary>
    public Emit MarkLabel(string name)
    {
        _innerEmit.MarkLabel(name);
        return this;
    }
}
