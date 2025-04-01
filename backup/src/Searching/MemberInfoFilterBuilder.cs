using MT = System.Reflection.MemberTypes;

namespace ScrubJay.Reflection.Searching;

public sealed class MemberInfoFilters : MemberInfoFilterBuilder<MemberInfoFilters, MemberInfo>
{
    public MemberInfoFilters(IEnumerable<MemberInfo> members) : base(members)
    {
    }
}


public abstract class MemberInfoFilterBuilder<TB, TM> : MemberBaseFilterBuilder<TB, TM>
    where TB : MemberInfoFilterBuilder<TB, TM>
    where TM : MemberInfo
{
    public FieldFilters Fields => new(MemberTypes(MT.Field).ItemsOfType<FieldInfo>());

    public PropertyFilters Properties => new(MemberTypes(MT.Property).ItemsOfType<PropertyInfo>());

    public EventFilters Events => new(MemberTypes(MT.Event).ItemsOfType<EventInfo>());

    public MethodFilters Methods => new(MemberTypes(MT.Method).ItemsOfType<MethodInfo>());

    public ConstructorFilters Constructors => new(MemberTypes(MT.Constructor).ItemsOfType<ConstructorInfo>());

    public TypeFilters Types => new(MemberTypes(MT.TypeInfo).ItemsOfType<Type>());

    protected MemberInfoFilterBuilder(IEnumerable<TM> members) : base(members)
    {
    }

    public MemberBaseFilters MemberTypes(MT memberTypes) => new(Where(m => memberTypes.HasAnyFlags(m.MemberType))._items);
}
