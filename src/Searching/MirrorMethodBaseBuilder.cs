namespace ScrubJay.Reflection.Searching;

public abstract class MirrorMethodBaseBuilder<B, M> : MirrorMemberBaseBuilder<B, M>
    where B : MirrorMethodBaseBuilder<B, M>
    where M : MethodBase
{
    protected MirrorMethodBaseBuilder(Type reflectedType, IEnumerable<M> members) 
        : base(reflectedType, members)
    {
    }

    public B NoParams => Where(static method => method.GetParameters().Length == 0);

    public B Parameters(params Type[]? parameterTypes)
    {
        if (parameterTypes == null)
            return Where(static method => method.GetParameters().Length == 0);
        
        return Where(
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
    }
    
    public B Parameters(Type[]? parameterTypes, TypeMatch match)
    {
        if (parameterTypes == null)
            return Where(static method => method.GetParameters().Length == 0);
        
        return Where(method =>
        {
            var methodParams = method.GetParameters();
            if (methodParams.Length != parameterTypes.Length)
                return false;
            for (int i = 0; i < methodParams.Length; i++)
            {
                if (methodParams[i].ParameterType.Matches(parameterTypes[i], match))
                    return false;
            }

            return true;
        });
    }
    
    public B Parameters<T1>() => Parameters(typeof(T1));
    public B Parameters<T1, T2>() => Parameters(typeof(T1), typeof(T2));
    public B Parameters<T1, T2, T3>() => Parameters(typeof(T1), typeof(T2), typeof(T3));
    public B Parameters<T1, T2, T3, T4>() => Parameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
    public B Parameters<T1, T2, T3, T4, T5>() => Parameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

    
    public B NotGeneric => Where(static method => method.GetGenericArguments().Length == 0);

    public B GenericTypes(params Type[]? types)
    {
        if (types == null)
            return Where(static method => method.GetGenericArguments().Length == 0);
        
        return Where(
            method =>
            {
                var genericTypes = method.GetGenericArguments();
                if (genericTypes.Length != types.Length)
                    return false;
                for (int i = 0; i < genericTypes.Length; i++)
                {
                    if (genericTypes[i] != types[i])
                        return false;
                }

                return true;
            });
    }
    
    public B GenericTypes(Type[]? types, TypeMatch match)
    {
        if (types == null)
            return Where(static method => method.GetGenericArguments().Length == 0);
        
        return Where(method =>
        {
            var genericTypes = method.GetGenericArguments();
            if (genericTypes.Length != types.Length)
                return false;
            for (int i = 0; i < genericTypes.Length; i++)
            {
                if (genericTypes[i].Matches(types[i], match))
                    return false;
            }

            return true;
        });
    }
    
    public B GenericTypes<T1>() => GenericTypes(typeof(T1));
    public B GenericTypes<T1, T2>() => GenericTypes(typeof(T1), typeof(T2));
    public B GenericTypes<T1, T2, T3>() => GenericTypes(typeof(T1), typeof(T2), typeof(T3));
    public B GenericTypes<T1, T2, T3, T4>() => GenericTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
    public B GenericTypes<T1, T2, T3, T4, T5>() => GenericTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
}