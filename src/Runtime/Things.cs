namespace ScrubJay.Reflection.Runtime;

// delegates

public abstract class Things
{
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