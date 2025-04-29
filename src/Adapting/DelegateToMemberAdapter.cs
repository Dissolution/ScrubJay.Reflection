using ScrubJay.Reflection.IL.Emission;

namespace ScrubJay.Reflection.Adapting;

public abstract class DelegateToMemberAdapter
{
    protected static DynamicILMethod<D> NewDynamicILMethod<D>(string? name = null)
        where D : Delegate
    {
        return new DynamicILMethod<D>(name);
    }

    protected static Exception GetError(MemberInfo member, DelegateInfo delInfo,
        string? additionalInfo = null,
        [CallerArgumentExpression(nameof(member))]
        string? memberName = null)
    {
        string exMessage = TextBuilder.New
            .Append("Cannot adapt ")
            .Render(delInfo)
            .Append(" to interact with ")
            .Append(member.MemberType)
            .Append(' ')
            .AppendMember(member)
            .IfNotNull(additionalInfo,
                static (tb, msg) => tb.Append(": ").Append(msg))
            .ToStringAndDispose();

        return new ArgumentException(exMessage, memberName)
        {
            Data =
            {
                { "Member", member },
                { "DelegateInfo", delInfo },
            },
        };
    }

    protected static bool AllIncomingSkippable(ReadOnlySpan<ParameterInfo> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.IsParams() || parameter.HasDefaultValue)
                continue;
            return false;
        }
        return true;
    }
}