using System.Reflection;

namespace ScrubJay.Reflection.Searching;

public class Mirror
{
    protected readonly MemberInfo[] _allMembers;
    
    public Type Type { get; }
    public IReadOnlyList<MemberInfo> AllMembers => _allMembers;

    internal Mirror(Type type)
    {
        this.Type = type;
        _allMembers = type.GetMembers(BF.Public | BF.NonPublic | BF.Instance | BF.Static | BF.FlattenHierarchy);
    }
}

public class Mirror<T> : Mirror
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    internal Mirror() : base(typeof(T)) { }
}