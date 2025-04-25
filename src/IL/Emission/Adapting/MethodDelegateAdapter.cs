namespace ScrubJay.Reflection.IL.Emission.Adapting;

public class MethodDelegateAdapter<D> : MemberDelegateAdapter<MethodDelegateAdapter<D>, MethodBase, D>
    where D : Delegate
{
    public override Result<D> TryAdapt(MethodBase member) => new NotImplementedException();
}