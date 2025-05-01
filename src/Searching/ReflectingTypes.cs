namespace ScrubJay.Reflection.Searching;

[PublicAPI]
public sealed class ReflectingTypes : ReflectingTypeInfos<ReflectingTypes>
{
    public ReflectingTypes(IEnumerable<Type> types) 
        : base(types)
    {
    }
}

[PublicAPI]
public abstract class ReflectingTypeInfos<B> : ReflectingMemberBases<B, Type>
    where B : ReflectingTypeInfos<B>
{
    protected ReflectingTypeInfos(IEnumerable<Type> types) 
        : base(types)
    {
    }

    public B NonGeneric => Only(static type => type.GetGenericArguments().Length == 0);

    public B GenericTypes(params Type[]? types)
    {
        if (types == null)
            return Only(static type => type.GetGenericArguments().Length == 0);

        return Only(types, static (type,ts) =>
            {
                var genericTypes = type.GetGenericArguments();
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
            return Only(static type => type.GetGenericArguments().Length == 0);

        return Only(types, match, static (type,ts,m) =>
        {
            var genericTypes = type.GetGenericArguments();
            if (genericTypes.Length != ts.Length)
                return false;
            for (int i = 0; i < genericTypes.Length; i++)
            {
                if (genericTypes[i].Matches(ts[i], m))
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