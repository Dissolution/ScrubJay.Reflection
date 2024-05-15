namespace ScrubJay.Reflection.Runtime.Naming;

public static class DelegateNames
{
    public static string Get<TDelegate>()
        where TDelegate : Delegate
        => TypeNames.Dump(typeof(TDelegate));
}