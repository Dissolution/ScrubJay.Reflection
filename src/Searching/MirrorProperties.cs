namespace ScrubJay.Reflection.Searching;

public enum SetKind
{
    Set,
    Init,
    Ctor,
}

public sealed class MirrorProperties : MirrorPropertyBuilder<MirrorProperties>
{
    internal MirrorProperties(Type reflectedType, IEnumerable<PropertyInfo> members) 
        : base(reflectedType, members)
    {
    }
}


public abstract class MirrorPropertyBuilder<B> : MirrorMemberBaseBuilder<B, PropertyInfo>
    where B : MirrorPropertyBuilder<B>
{
    protected MirrorPropertyBuilder(Type reflectedType, IEnumerable<PropertyInfo> members) 
        : base(reflectedType, members)
    {
    }

    public B Returning(Type type)
    {
        return Where(prop => prop.PropertyType == type);
    }

    public B Returning(Type type, TypeMatch match)
    {
        return Where(field => field.PropertyType.Matches(type, match));
    }

    public B Returning<T>() => Returning(typeof(T));

    public B Returning<T>(TypeMatch match) => Returning(typeof(T), match);

    public B Gettable()
    {
        return Where(prop => prop.GetMethod is not null);
    }

    public B Gettable(bool gettable)
    {
        if (gettable)
        {
            return Where(static prop => prop.GetMethod is not null);
        }
        else
        {
            return Where(static prop => prop.GetMethod is null);
        }
    }

    public B Gettable(Viz visibility)
    {
        return Where(prop => prop.GetMethod is not null &&
            prop.GetMethod.Visibility().HasFlags(visibility));
    }
    
    
    public B Settable()
    {
        return Where(prop => prop.SetMethod is not null);
    }

    public B Settable(bool settable)
    {
        if (settable)
        {
            return Where(static prop => prop.SetMethod is not null);
        }
        else
        {
            return Where(static prop => prop.SetMethod is null);
        }
    }

    public B Settable(Viz visibility)
    {
        return Where(prop => prop.SetMethod is not null &&
            prop.SetMethod.Visibility().HasFlags(visibility));
    }
    
    public B Settable(SetKind kind) => kind switch
    {
        SetKind.Set => Settable(true),
        SetKind.Init => Where(static prop => prop.IsInitOnly()),
        SetKind.Ctor => Settable(false),
        _ => throw InvalidEnumException.Create(kind),
    };

    public B NotAnIndexer
    {
        get
        {
            return Where(static prop => prop.GetIndexParameters().Length == 0);
        }
    }
    
    public B Indexer(params Type[]? types)
    {
        if (types == null)
            return Where(static property => property.GetIndexParameters().Length == 0);
        
        return Where(
            property =>
            {
                var indexerParameters = property.GetIndexParameters();
                if (indexerParameters.Length != types.Length)
                    return false;
                for (int i = 0; i < indexerParameters.Length; i++)
                {
                    if (indexerParameters[i].ParameterType != types[i])
                        return false;
                }

                return true;
            });
    }
    
    public B Indexer(Type[]? types, TypeMatch match)
    {
        if (types == null)
            return Where(static property => property.GetIndexParameters().Length == 0);
        
        return Where(property =>
        {
            var indexerParameters = property.GetIndexParameters();
            if (indexerParameters.Length != types.Length)
                return false;
            for (int i = 0; i < indexerParameters.Length; i++)
            {
                if (indexerParameters[i].ParameterType.Matches(types[i], match))
                    return false;
            }

            return true;
        });
    }
    
    public B Indexer<T1>() => Indexer(typeof(T1));
    public B Indexer<T1, T2>() => Indexer(typeof(T1), typeof(T2));
    public B Indexer<T1, T2, T3>() => Indexer(typeof(T1), typeof(T2), typeof(T3));
    public B Indexer<T1, T2, T3, T4>() => Indexer(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
    public B Indexer<T1, T2, T3, T4, T5>() => Indexer(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));


}