namespace ScrubJay.Reflection.Searching;

public sealed class MirrorTypes : MirrorTypesBuilder<MirrorTypes>
{
    internal MirrorTypes(Type reflectedType, IEnumerable<Type> members) : base(reflectedType, members)
    {
    }
}

public abstract class MirrorTypesBuilder<B> : MirrorMemberBaseBuilder<B, Type>
    where B : MirrorTypesBuilder<B>
{
    protected MirrorTypesBuilder(Type reflectedType, IEnumerable<Type> members) : base(reflectedType, members)
    {
    }

    public B NotGeneric => Where(static type => type.GetGenericArguments().Length == 0);

    public B GenericTypes(params Type[]? types)
    {
        if (types == null)
            return Where(static type => type.GetGenericArguments().Length == 0);

        return Where(
            type =>
            {
                var genericTypes = type.GetGenericArguments();
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
            return Where(static type => type.GetGenericArguments().Length == 0);

        return Where(type =>
        {
            var genericTypes = type.GetGenericArguments();
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