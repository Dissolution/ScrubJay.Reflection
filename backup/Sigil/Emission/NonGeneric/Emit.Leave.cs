
namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Leave an exception or catch block, branching to the given label.
    ///
    /// This instruction empties the stack.
    /// </summary>
    public Emit Leave(SigilLabel sigilLabel)
    {
        _innerEmit.Leave(sigilLabel);
        return this;
    }

    /// <summary>
    /// Leave an exception or catch block, branching to the label with the given name.
    ///
    /// This instruction empties the stack.
    /// </summary>
    public Emit Leave(string name)
    {
        _innerEmit.Leave(name);
        return this;
    }
}
