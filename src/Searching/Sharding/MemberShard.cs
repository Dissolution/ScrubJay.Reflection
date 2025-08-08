using System.Linq.Expressions;
using ScrubJay.Reflection.Collections;
using ScrubJay.Text.Comparison;

namespace ScrubJay.Reflection.Searching.Sharding;

/// <summary>
/// Common to <em>all</em> shards
/// </summary>
/// <typeparam name="S"></typeparam>
/// <typeparam name="M"></typeparam>
public abstract class MemberShard<S, M> :
    FluentBuilderBase<S>,
    IRenderable
    where S : MemberShard<S, M>
    where M : MemberInfo
{
    protected readonly DLCL<M> _members;
    protected readonly List<string> _filters;

    public int Count => _members.Count;

    protected MemberShard(
        DLCL<M> members,
        List<string> filters
    ) : base()
    {
        _members = members;
        _filters = filters;
    }

    protected S AddFilter(
        Func<M, bool> predicate,
        ref InterpolatedTextBuilder interpolatedText)
    {
        var node = _members.First;
        while (node is not null)
        {
            if (predicate(node.Value))
            {
                node = node.Next;
            }
            else
            {
                node = node.DeleteAndNext();
            }
        }

        string str = interpolatedText.ToStringAndDispose();
        _filters.Add(str);
        return (S)this;
    }

    protected S AddFilter(
        Func<M, bool> predicate,
        string text)
    {
        var node = _members.First;
        while (node is not null)
        {
            if (predicate(node.Value))
            {
                node = node.Next;
            }
            else
            {
                node = node.DeleteAndNext();
            }
        }

        _filters.Add(text);
        return (S)this;
    }

    protected DLCL<T> AddTypeFilter<T>()
        where T : MemberInfo
    {
        DLCL<T> newlist = new();

        var node = _members.First;
        while (node is not null)
        {
            if (node.Value is T typed)
            {
                newlist.AddLast(typed);
            }

            node = node.Next;
        }

        return newlist;
    }

#region Visibility | BindingFlags

    public S Visibility(Viz visibility)
    {
        return AddFilter(
            m => m.Visibility() == visibility,
            $"Visibility == {visibility:@}");
    }

    public S Public() => Visibility(Viz.Public);
    
    public S NonPublic() => Visibility(Viz.NonPublic);
    
    public S Internal() => Visibility(Viz.Internal);
    
    public S Protected() => Visibility(Viz.Protected);

    public S Private() => Visibility(Viz.Private);

    public S Instance() => Visibility(Viz.Instance);

    public S Static() => Visibility(Viz.Static);

    public S BindingFlags(BF bindingFlags)
    {
        return AddFilter(
            m => m.BindingFlags().HasFlags(bindingFlags),
            $"BindingFlags == {bindingFlags:@}");
    }

#endregion

#region Name

    public S Named(string name)
    {
        return AddFilter(
            m => m.Name.Equate(name),
            $"Name == \"{name}\"");
    }

    public S Named(string name, StringComparison comparison)
    {
        return AddFilter(
            m => m.Name.Equate(name, comparison),
            $"Name == \"{name}\" ({comparison:@})");
    }

    public S Named(string name, StringMatch match)
    {
        return AddFilter(
            m => m.Name.Matches(name, match),
            $"Name == {match:@}");
    }

#endregion

#region Attributes

    public S HasAttribute<A>()
        where A : Attribute
    {
        return AddFilter(m => m.HasAttribute<A>(), $"Attributes ⊃ {typeof(A):@}");
    }

    public S HasAttribute<A>(Expression<Func<A, bool>> attributePredicate)
        where A : Attribute
    {
        return AddFilter(
            m => ((A[])Attribute.GetCustomAttributes(m, typeof(A))).Any(attributePredicate.Compile()),
            $"Attributes ⊃ {typeof(A):@} ~ {attributePredicate:@}");
    }

    public S HasAttribute(Type attributeType)
    {
        return AddFilter(m => m.HasAttribute(attributeType), $"Attributes ⊃ {attributeType:@}");
    }

#endregion

    public S Where(Expression<Func<M, bool>> memberPredicate)
    {
        var func = memberPredicate.Compile();
        return AddFilter(member => func(member), memberPredicate.Render());
    }

    public IEnumerable<M> ToEnumerable()
    {
        return _members;
    }

    public List<M> ToList()
    {
        return _members.ToList();
    }

    public bool TryGetOnly([NotNullWhen(true)] out M? member)
    {
        if (_members.Count == 1)
        {
            member = _members._head!._value;
            return true;
        }

        member = default;
        return false;
    }

    public Result<M> TryGetOnly()
    {
        if (_members.Count == 1)
        {
            return _members._head!._value;
        }

        string message = TextBuilder.New
            .AppendLine($"Filters reduced to {Count} item(s):")
            .Enumerate(_filters, static (tb, filter) => tb.AppendLine(filter))
            .ToStringAndDispose();

        return new MissingMemberException(message);
    }

    public M OneOrThrow() => TryGetOnly().OkOrThrow();

    public virtual void RenderTo(TextBuilder builder)
    {
        builder.Enumerate(_filters, static (tb, filter) => tb.Append(filter).NewLine());

        if (_members.Count == 0)
        {
            builder.Append("[]");
            return;
        }

        builder.AppendLine('[')
            .Enumerate(_members, static (tb, member) => tb.Render(member).NewLine())
            .Append(']');
    }

    public override sealed string ToString() => this.Render();
}