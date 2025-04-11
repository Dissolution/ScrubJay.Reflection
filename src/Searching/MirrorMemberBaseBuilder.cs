using ScrubJay.Text.Comparison;

namespace ScrubJay.Reflection.Searching;

public abstract class MirrorMemberBaseBuilder<B, M> : FluentListBuilder<B, M>
    where B : MirrorMemberBaseBuilder<B, M>
    where M : MemberInfo
{
    public Type ReflectedType { get; }

    public B Public => Visibility(Viz.Public);
    public B NonPublic => Visibility(Viz.NonPublic);
    public B Internal => Visibility(Viz.Internal);
    public B Protected => Visibility(Viz.Protected);
    public B Private => Visibility(Viz.Private);

    public B Instance => Visibility(Viz.Instance);
    public B Static => Visibility(Viz.Static);

    protected MirrorMemberBaseBuilder(Type reflectedType, IEnumerable<M> members)
        : base(members)
    {
        Throw.IfNull(reflectedType);
        this.ReflectedType = reflectedType;
    }

    public B Visibility(Viz visibility)
    {
        return Where(member => member.Visibility().HasFlags(visibility));
    }

    public B BindingFlags(BF bindingFlags)
    {
        return Where(member => member.BindingFlags().HasFlags(bindingFlags));
    }

    public B Named(string name)
    {
        return Where(member => TextHelper.Equate(member.Name, name));
    }

    public B Named(string? name, StringMatch match)
    {
        return Where(member => member.Name.Matches(name, match));
    }

    public B With(Type attributeType)
    {
        Throw.IfNull(attributeType);
        if (!attributeType.Implements<Attribute>())
            throw new ArgumentException($"{attributeType.NameOf()} does not implement Attribute", nameof(attributeType));
        return Where(member => member.HasAttribute(attributeType));
    }

    public B With<A>()
        where A : Attribute
    {
        return Where(member => member.HasAttribute<A>());
    }
    
    public override string ToString() => TextBuilder.New
        .Append('[')
        .If(Validate.IsNotEmpty<M>(_values),
            (tb, members) => tb
                .NewLine()
                .Enumerate(members, static (t, m) => t.Append("    ").AppendMember(m).NewLine()))
        .Append(']')
        .ToStringAndDispose();
}