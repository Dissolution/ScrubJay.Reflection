using ScrubJay.Reflection.Extensions;

namespace ScrubJay.Reflection.Searching;

public class MemberFilterBuilder<B, M> : FilterBuilder<B, M>
    where B : MemberFilterBuilder<B, M>
    where M : MemberInfo
{
    public MemberFilterBuilder(IEnumerable<M> members) : base(members)
    {
    }

    public B Visibility(Visibility visibility)
        => Where(m => visibility.HasAnyFlags(m.Visibility()));
}