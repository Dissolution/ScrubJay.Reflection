namespace ScrubJay.Reflection.Searching;

public sealed class MirrorMethodBases : ReflectingMethodBases<MirrorMethodBases, MethodBase>
{
    public MirrorMethodBases(IEnumerable<MethodBase> methods) 
        : base(methods)
    {
        
    }
}

public abstract class ReflectingMethodBases<B, M> : ReflectingMemberBases<B, M>
    where B : ReflectingMethodBases<B, M>
    where M : MethodBase
{
    protected ReflectingMethodBases(IEnumerable<M> methods) 
        : base(methods)
    {
    }

    public B NoParams => Only(static method => method.GetParameters().Length == 0);

    public B ParamCount(int count) => Only(count, static (method, cnt) => method.GetParameters().Length == cnt);
    
    public B Parameters(params Type[]? parameterTypes)
    {
        if (parameterTypes == null)
            return Only(static method => method.GetParameters().Length == 0);
        
        return Only(parameterTypes, 
            static (method,pTypes) =>
            {
                var methodParams = method.GetParameters();
                if (methodParams.Length != pTypes.Length)
                    return false;
                for (int i = 0; i < methodParams.Length; i++)
                {
                    if (methodParams[i].ParameterType != pTypes[i])
                        return false;
                }

                return true;
            });
    }
    
    public B Parameters(Type[]? parameterTypes, TypeMatch match)
    {
        if (parameterTypes == null)
            return Only(static method => method.GetParameters().Length == 0);
        
        return Only(parameterTypes, match, static (method,pTypes, m) =>
        {
            var methodParams = method.GetParameters();
            if (methodParams.Length != pTypes.Length)
                return false;
            for (int i = 0; i < methodParams.Length; i++)
            {
                if (methodParams[i].ParameterType.Matches(pTypes[i], m))
                    return false;
            }

            return true;
        });
    }

    public B Accepting(params object?[] args)
    {
        return Only(args, static (method,a) =>
        {
            var methodParams = method.GetParameters();
            var paramCount = methodParams.Length;
            if (paramCount < a.Length)
                return false;

            for (int p = 0; p < paramCount; p++)
            {
                var param = methodParams[p];
                if (p < a.Length)
                {
                    var arg = a[p];
                    if (!param.CanAccept(arg))
                        return false;
                }
                else
                {
                    if (!param.Default().IsSome())
                        return false;
                }
            }

            return true;
        });
    }
    
    public B Parameters<T1>() => Parameters(typeof(T1));
    public B Parameters<T1, T2>() => Parameters(typeof(T1), typeof(T2));
    public B Parameters<T1, T2, T3>() => Parameters(typeof(T1), typeof(T2), typeof(T3));
    public B Parameters<T1, T2, T3, T4>() => Parameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
    public B Parameters<T1, T2, T3, T4, T5>() => Parameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

    
    public B NotGeneric => Only(static method => method.GetGenericArguments().Length == 0);

    public B IsGeneric => Only(static method => method.IsGenericMethod);

    public B GenericCount(int count) 
        => Only(count, static (method,cnt) => method.GetGenericArguments().Length == cnt);
    
    public B GenericTypes(params Type[]? types)
    {
        if (types == null)
            return Only(static method => method.GetGenericArguments().Length == 0);
        
        return Only(types, static (method, ts) =>
            {
                var genericTypes = method.GetGenericArguments();
                if (genericTypes.Length != ts.Length)
                    return false;
                for (int i = 0; i < genericTypes.Length; i++)
                {
                    if (genericTypes[i] != ts[i])
                        return false;
                }

                return true;
            });
    }
    
    public B GenericTypes(Type[]? types, TypeMatch match)
    {
        if (types == null)
            return Only(static method => method.GetGenericArguments().Length == 0);
        
        return Only(types, match, static (method,t,m) =>
        {
            var genericTypes = method.GetGenericArguments();
            if (genericTypes.Length != t.Length)
                return false;
            for (int i = 0; i < genericTypes.Length; i++)
            {
                if (genericTypes[i].Matches(t[i], m))
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
    
    
    public B Returning(Type type)
    {
        return Only(type, static (method, t) => method.ReturnType() == t);
    }

    public B Returning(Type type, TypeMatch match)
    {
        return Only(type, match, static (method,t,m) => method.ReturnType().Matches(t,m));
    }

    public B Returning<T>() => Returning(typeof(T));
    
    public B Returning<T>(TypeMatch match) => Returning(typeof(T), match); 
    
}