namespace ScrubJay.Reflection.Adapting;

[PublicAPI]
public interface IDelegateToMemberAdapter<in M>
    where M : MemberInfo
{
#if NET7_0_OR_GREATER
    static abstract Result<D> TryAdapt<D>(M member)
        where D : Delegate;
#endif
}