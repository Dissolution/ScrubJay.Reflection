using ScrubJay.Reflection.Info;

namespace ScrubJay.Reflection.Runtime;

public class DelegateBuilder<TDelegate> : DelegateBuilder
    where TDelegate : Delegate
{
    public DelegateBuilder(string? name = null) : base(DelegateSignature.For<TDelegate>(name))
    {

    }

    public new TDelegate CreateDelegate()
    {
#if NET6_0_OR_GREATER
        return _dynamicMethod.CreateDelegate<TDelegate>();
#else
        return (TDelegate)_dynamicMethod.CreateDelegate(typeof(TDelegate));
#endif
    }
}