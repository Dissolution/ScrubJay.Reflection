namespace ScrubJay.Sigil;

/// <summary>
/// Provides a way to lookup locals in scope by name.
/// </summary>
public class LocalLookup
{
    /// <summary>
    /// Returns the local with the given name.
    ///
    /// Throws KeyNotFoundException if no local by that name is found".
    /// </summary>
    public SigilLocal this[string name]
    {
        get
        {
            if (!_innerLookup.ContainsKey(name))
            {
                throw new KeyNotFoundException("No local with name '" + name + "' found");
            }

            return _innerLookup[name];
        }
    }

    /// <summary>
    /// Returns the number of locals in scope
    /// </summary>
    public int Count { get { return _names.Count(); } }

    // ReSharper disable once InconsistentNaming
    private IEnumerable<string> _names { get { return _innerLookup.Keys.Where(k => !k.StartsWith("__")).ToList(); } }

    /// <summary>
    /// Returns the names of all the locals in scope
    /// </summary>
    public IEnumerable<string> Names { get { return _names; } }

    private Dictionary<string, SigilLocal> _innerLookup;

    internal LocalLookup(Dictionary<string, SigilLocal> innerLookup)
    {
        _innerLookup = innerLookup;
    }
}
