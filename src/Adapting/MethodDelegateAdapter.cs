namespace ScrubJay.Reflection.Adapting;

public class MethodDelegateAdapter<D> : MemberDelegateAdapter<MethodDelegateAdapter<D>, MethodBase, D>
    where D : Delegate
{
    public override Result<D> TryAdapt(MethodBase member) => new NotImplementedException();
}