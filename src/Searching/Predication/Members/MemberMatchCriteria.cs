using Vis = ScrubJay.Reflection.Visibility;
using Acc = ScrubJay.Reflection.Access;

namespace ScrubJay.Reflection.Searching.Predication.Members;

public record class MemberMatchCriteria<TMember> : MatchCriteria<TMember>
    where TMember : MemberInfo
{
    public Vis? Visibility { get; set; }
    public Acc? Access { get; set; }
    public MemberTypes? MemberTypes { get; set; }
    public ICriteria<string>? Name { get; set; }

    public ICriteria<Attribute[]>? Attributes { get; set; }
    public ICriteria<Type?>? DeclaringType { get; set; }

    public BindingFlags BindingFlags()
    {
        BindingFlags flags = default;
        if (Visibility.TryGetValue(out var visibility))
        {
            if (visibility.HasAnyFlags(Vis.Private, Vis.Protected, Vis.Internal))
                flags |= System.Reflection.BindingFlags.NonPublic;
            if (visibility.HasFlags(Vis.Public))
                flags |= System.Reflection.BindingFlags.Public;
        }

        if (Access.TryGetValue(out var access))
        {
            if (access.HasFlags(Acc.Static))
                flags |= System.Reflection.BindingFlags.Static;
            if (access.HasFlags(Acc.Instance))
                flags |= System.Reflection.BindingFlags.Instance;
        }

        return flags;
    }
    
    public override bool Matches([NotNullWhen(true)] TMember? member)
    {
        // By default, we do not match null
        if (member is null)
            return false;

        if (Visibility.TryGetValue(out var visibility) && !visibility.HasAnyFlags(member.Visibility()))
            return false;
        
        if (Access.TryGetValue(out var access) && !access.HasAnyFlags(member.Access()))
            return false;

        if (MemberTypes.TryGetValue(out var memberTypes) && !memberTypes.HasAnyFlags(member.MemberType))
            return false;

        if (Name is not null && !Name.Matches(member.Name))
            return false;

        if (Attributes is not null)
        {
            var attributes = Attribute.GetCustomAttributes(member);
            if (!Attributes.Matches(attributes))
                return false;
        }
        
        if (DeclaringType is not null && !DeclaringType.Matches(member.DeclaringType))
            return false;

        return true;
    }
}