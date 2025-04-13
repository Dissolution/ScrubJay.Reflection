namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class MethodBaseExtensions
{
    /// <summary>
    /// Can this <see cref="MethodBase"/> be overriden?
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    /// <see href="https://stackoverflow.com/questions/38078948/check-if-a-classes-property-or-method-is-declared-as-sealed"/>
    public static bool IsOverridable(this MethodBase method) => method.IsVirtual && !method.IsFinal;

    /// <summary>
    /// Is this <see cref="MethodBase"/> <c>sealed</c>?
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsSealed(this MethodBase method) => method.IsFinal || !method.IsVirtual;

    public static bool IsAsync(this MethodBase? method)
    {
        if (method is null)
            return false;
        return typeof(IAsyncStateMachine).IsAssignableFrom(method.DeclaringType);
    }
    
    
    
    /// <summary>
    /// Get the <see cref="Type">Types</see> of the parameters in this <see cref="MethodBase"/>
    /// </summary>
    public static Type[] GetParameterTypes(this MethodBase method)
    {
        var parameters = method.GetParameters();
        Type[] types = new Type[parameters.Length];
        // reverse to elide bounds checks
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
            ConstructorInfo ctor => ctor.OwnerType(),
            _ => throw new ArgumentException("Invalid Method", nameof(method)),
        };
    }
}