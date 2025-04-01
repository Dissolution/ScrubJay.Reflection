namespace ScrubJay.Reflection.Extensions;

public static class MethodBaseExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    /// <see href="https://stackoverflow.com/questions/38078948/check-if-a-classes-property-or-method-is-declared-as-sealed"/>
    public static bool IsOverridable(this MethodBase method) => method.IsVirtual && !method.IsFinal;

    public static bool IsSealed(this MethodBase method) => !IsOverridable(method);

    /// <summary>
    /// Get the <see cref="Type">Types</see> of the parameters in this <see cref="MethodBase"/>
    /// </summary>
    public static Type[] GetParameterTypes(this MethodBase method)
    {
        var parameters = method.GetParameters();
        Type[] types = new Type[parameters.Length];
        for (var i = parameters.Length - 1; i >= 0; i--)
        {
            types[i] = parameters[i].ParameterType;
        }
        return types;
    }

    /// <summary>
    /// Gets the <see cref="Type"/> returned by this <see cref="MethodBase"/>
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Type ReturnType(this MethodBase method)
    {
        return method switch
        {
            MethodInfo info => info.ReturnType,
            ConstructorInfo { IsStatic: true } => typeof(void),
            ConstructorInfo ctor => ctor.DeclaringType ?? ctor.Module.GetType(),
            _ => throw new ArgumentException("Invalid Method", nameof(method)),
        };
    }
}