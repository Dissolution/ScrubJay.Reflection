namespace ScrubJay.Sigil;

/// <summary>
/// Provides a way to lookup labels declared with an emit.
/// </summary>
public class LabelLookup
{
    /// <summary>
    /// Returns the label with the given name.
    ///
    /// Throws KeyNotFoundException if no label by that name is found".
    /// </summary>
    public SigilLabel this[string name]
    {
        get
        {
            if (!_innerLookup.ContainsKey(name))
            {
                throw new KeyNotFoundException("No label with name '" + name + "' found");
            }

            return _innerLookup[name];
        }
    }

    /// <summary>
    /// Returns the number of labels declared
    /// </summary>
    public int Count { get { return _names.Count(); } }

    // ReSharper disable once InconsistentNaming
    private IEnumerable<string> _names { get { return _innerLookup.Keys.Where(k => !k.StartsWith("__")).ToList(); } }

    /// <summary>
    /// Returns the names of all the declared labels
    /// </summary>
    public IEnumerable<string> Names { get { return _names; } }

    private Dictionary<string, SigilLabel> _innerLookup;

    internal LabelLookup(Dictionary<string, SigilLabel> innerLookup)
    {
        _innerLookup = innerLookup;
    }
}
