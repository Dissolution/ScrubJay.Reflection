namespace ScrubJay.Reflection.Searching;

public abstract class MethodBaseFilterBuilder<TB, TM> : MemberBaseFilterBuilder<TB, TM>
    where TB : MethodBaseFilterBuilder<TB, TM>
    where TM : MethodBase
{
    protected MethodBaseFilterBuilder(IEnumerable<TM> methods) : base(methods)
    {
    }

    public TB NoParameters => Where(m => m.GetParameters().Length == 0);

    public TB Parameters(object?[] parameters, TypeMatch typeMatch = TypeMatch.ImplementedBy) => Where(
            method =>
            {
                var methodParams = method.GetParameters();
                if (methodParams.Length != parameters.Length)
                    return false;
                for (int i = 0; i < methodParams.Length; i++)
                {
                    var pType = parameters[i]?.GetType() ?? typeof(object);
                    if (!methodParams[i].ParameterType.Equals(pType, typeMatch))
                        return false;
                }

                return true;
            });

    public TB ParameterTypes(params Type[] parameterTypes) => Where(
            method =>
            {
                var methodParams = method.GetParameters();
                if (methodParams.Length != parameterTypes.Length)
                    return false;
                for (int i = 0; i < methodParams.Length; i++)
                {
                    if (methodParams[i].ParameterType != parameterTypes[i])
                        return false;
                }

                return true;
            });

    public TB ParameterTypes(Type[] parameterTypes, TypeMatch typeMatch) => Where(
            method =>
            {
                var methodParams = method.GetParameters();
                if (methodParams.Length != parameterTypes.Length)
                    return false;
                for (int i = 0; i < methodParams.Length; i++)
                {
                    if (!methodParams[i].ParameterType.Equals(parameterTypes[i], typeMatch))
                        return false;
                }

                return true;
            });

    public TB ParameterTypes<T1>() => ParameterTypes(typeof(T1));
    public TB ParameterTypes<T1, T2>() => ParameterTypes(typeof(T1), typeof(T2));
    public TB ParameterTypes<T1, T2, T3>() => ParameterTypes(typeof(T1), typeof(T2), typeof(T3));
    public TB ParameterTypes<T1, T2, T3, T4>() => ParameterTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
    public TB ParameterTypes<T1, T2, T3, T4, T5>() => ParameterTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

    public TB GenericTypes(params Type[] genericTypes)
        => Where(m => Sequence.Equal(m.GetGenericArguments(), genericTypes));
}
