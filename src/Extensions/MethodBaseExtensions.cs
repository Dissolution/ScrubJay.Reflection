using ScrubJay.Validation;

namespace ScrubJay.Reflection.Extensions;

public static class MethodBaseExtensions
{
    public static Type[] GetParameterTypes(this MethodBase method)
    {
        var parameters = method.GetParameters();
        Type[] types = new Type[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            types[i] = parameters[i].ParameterType;
        }
        return types;
    }

    public static Type ReturnType(this MethodBase method)
    {
        if (method is MethodInfo info)
        {
            return info.ReturnType;
        }
        if (method is ConstructorInfo ctor)
        {
            if (ctor.IsStatic) return typeof(void);
            return ctor.DeclaringType.ThrowIfNull("Constructor does not have a declaring type");
        }
        throw new ArgumentException("Invalid Method", nameof(method));
    }
}