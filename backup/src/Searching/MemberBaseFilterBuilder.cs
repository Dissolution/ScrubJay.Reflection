using ScrubJay.Text.Comparison;


namespace ScrubJay.Reflection.Searching;

public sealed class MemberBaseFilters : MemberBaseFilterBuilder<MemberBaseFilters, MemberInfo>
{
    public MemberBaseFilters(IEnumerable<MemberInfo> members) : base(members)
    {
    }
}

public class MemberBaseFilterBuilder<TB, TM> : FilterBuilder<TB, TM>
    where TB : MemberBaseFilterBuilder<TB, TM>
    where TM : MemberInfo
{
    public TB Public => Visibility(ScrubJay.Reflection.Visibility.Public);
    public TB Internal => Visibility(ScrubJay.Reflection.Visibility.Internal);
    public TB Protected => Visibility(ScrubJay.Reflection.Visibility.Protected);
    public TB Private => Visibility(ScrubJay.Reflection.Visibility.Private);
    public TB NonPublic => Visibility(ScrubJay.Reflection.Visibility.NonPublic);

    public TB Instance => Access(ScrubJay.Reflection.Access.Instance);
    public TB Static => Access(ScrubJay.Reflection.Access.Static);

    public MemberBaseFilterBuilder(IEnumerable<TM> members) : base(members)
    {
    }

    public TB Visibility(Visibility visibility) => Where(m => visibility.HasAnyFlags(m.Visibility()));

    public TB Access(Access access) => Where(m => access.HasAnyFlags(m.Access()));

    public TB BindingFlags(BindingFlags bindingFlags) => Where(m => bindingFlags.HasAnyFlags(m.BindingFlags()));

    public TB HasAttribute(Type attributeType) => Where(m => Attribute.IsDefined(m, attributeType));

    public TB HasAttribute<TA>()
        where TA : Attribute
        => Where(m => Attribute.IsDefined(m, typeof(TA)));


    public TB Named(string exactName) => Where(m => m.Name == exactName);

    public TB Named(string exactName, StringComparison comparison) => Where(m => string.Equals(m.Name, exactName, comparison));

    public TB Named(string exactName, StringMatch match) => Where(m => m.Name.Matches(exactName, match));
}
