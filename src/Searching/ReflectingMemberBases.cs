using ScrubJay.Text.Comparison;

namespace ScrubJay.Reflection.Searching;

[PublicAPI]
public abstract class ReflectingMemberBases<B, M> : FluentListBuilder<B, M>, IRenderable
    where B : ReflectingMemberBases<B, M>
    where M : MemberInfo
{
    public B Public => Visibility(Viz.Public);
    public B NonPublic => Visibility(Viz.NonPublic);
    public B Internal => Visibility(Viz.Internal);
    public B Protected => Visibility(Viz.Protected);
    public B Private => Visibility(Viz.Private);

    public B Instance => Visibility(Viz.Instance);
    public B Static => Visibility(Viz.Static);

    protected ReflectingMemberBases(IEnumerable<M> members)
        : base(members)
    {
    }

    public B Visibility(Viz visibility)
    {
        return Only(visibility, static (member, viz) => member.Visibility().HasFlags(viz));
    }

    public B BindingFlags(BF bindingFlags)
    {
        return Only(bindingFlags, static (member, flags) => member.BindingFlags().HasFlags(flags));
    }

    public B Named(string name)
    {
        return Only(name, static (member, n) => TextHelper.Equate(member.Name, n));
    }

    public B Named(string name, StringComparison comparison)
    {
        return Only(name, comparison, static (member, n, c) => TextHelper.Equate(member.Name, n, c));
    }

    public B Named(string? name, StringMatch match)
    {
        return Only(name, match, static (member, n, m) => member.Name.Matches(n, m));
    }

    public B Attr(Type attributeType)
    {
        Throw.IfNull(attributeType);
        if (!attributeType.Implements<Attribute>())
            throw new ArgumentException($"{attributeType.NameOf()} does not implement Attribute", nameof(attributeType));
        return Only(attributeType, static (member, at) => member.HasAttribute(at));
    }
    
    public B Attr<A>()
        where A : Attribute
        => Attr(typeof(A));

    public B Attr<A>(Func<A, bool> predicate)
        where A : Attribute
    {
        return Only(typeof(A), predicate, (m, t, f) =>
        {
            var attributes = Attribute.GetCustomAttributes(m, t);
            return attributes.OfType<A>().Any(predicate);
        });
    }

    public virtual void RenderTo<TB>(TB builder) 
        where TB : TextBuilderBase<TB>
    {
        builder.Append('[')
            .If(Validate.IsNotEmpty<M>(_values),
                (tb, members) => tb
                    .NewLine()
                    .Enumerate(members, static (t, m) => t.Append("    ").AppendMember(m).NewLine()))
            .Append(']');
    }

    public sealed override string ToString() => TextBuilder.Build(RenderTo);
}