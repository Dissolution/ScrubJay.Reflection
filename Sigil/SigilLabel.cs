namespace ScrubJay.Sigil;

/// <summary>
/// Represents a Label in a CIL stream, and thus a Leave and Branch target.
///
/// To create a Label call DefineLabel().
///
/// Before creating a delegate, all Labels must be marked.  To mark a label, call MarkLabel().
/// </summary>
public class SigilLabel : IOwned
{
    /// <summary>
    /// The name of this Label.
    ///
    /// If one is omitted during creation a random one is created instead.
    ///
    /// Names are purely for debugging aid, and will not appear in the generated delegate.
    /// </summary>
    public string Name { get; private set; }

    internal DefineLabelDelegate LabelDel { get; private set; }

    private object _owner;
    object IOwned.Owner { get { return _owner; } }

    internal SigilLabel(object owner, DefineLabelDelegate label, string name)
    {
        _owner = owner;
        Name = name;
        LabelDel = label;
    }

    internal void SetOwner(object owner)
    {
        if (_owner != null && owner != null) throw new Exception("Cannot set ownership of an owner Label");

        _owner = owner;
    }

    /// <summary>
    /// Equivalent to Name.
    /// </summary>
    public override string ToString()
    {
        return Name;
    }
}
