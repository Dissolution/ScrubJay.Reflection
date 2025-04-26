namespace ScrubJay.Reflection.Adapting;

public abstract class MemberDelegateAdapter<S, M, D>
    where S : MemberDelegateAdapter<S, M, D>
    where M : MemberInfo
    where D : Delegate
{
    public static S Instance { get; } = Activator.CreateInstance<S>();
    
    public abstract Result<D> TryAdapt(M member);

    protected static Exception GetEx(M member,
        string? message = null,
        [CallerArgumentExpression(nameof(member))]
        string? memberName = null)
    {
        string exMessage = TextBuilder.New
            .Append("Cannot adapt ")
            .AppendType(typeof(D))
            .Append(" to interact with ")
            .Append(member.MemberType)
            .Append(' ')
            .AppendMember(member)
            .IfNotNull(message,
                static (tb, msg) => tb.Append(": ").Append(msg))
            .ToStringAndDispose();

        return new ArgumentException(exMessage, memberName)
        {
            Data =
            {
                { "MemberType", typeof(M) },
                { "Member", member },
                { "DelegateType", typeof(D) },
            },
        };
    }


    protected static bool IsAssertStatic(MemberInfo member, Type instanceType)
    {
        if (member.IsStatic())
        {
            if (instanceType == typeof(Unit) || instanceType == typeof(None))
            {
                return true;
            }
            throw new ArgumentException();
        }
        return false;
    }
}