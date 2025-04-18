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

    public B Contains(Type type)
    {
        return Only(type, static (prop,t) => prop.PropertyType == t);
    }

    public B Contains(Type type, TypeMatch match)
    {
        return Only(type, match,
            static (field,t,m) => field.PropertyType.Matches(t,m));
    }

    public B Contains<T>() => Contains(typeof(T));

    public B Contains<T>(TypeMatch match) => Contains(typeof(T), match);

    public B Gettable()
    {
        return Only(prop => prop.GetMethod is not null);
    }

    public B Gettable(bool gettable)
    {
        return Only(gettable, static (prop, g) => (prop.GetMethod is not null) == g);
    }

    public B Gettable(Viz visibility)
    {
        return Only(visibility, static (prop,viz) => prop.GetMethod is not null &&
            prop.GetMethod.Visibility().HasFlags(viz));
    }
    
    
    public B Settable()
    {
        return Only(prop => prop.SetMethod is not null);
    }

    public B Settable(bool settable)
    {
        return Only(settable, static (prop, s) => (prop.SetMethod is not null) == s);
    }

    public B Settable(Viz visibility)
    {
        return Only(visibility, static (prop,viz) => prop.SetMethod is not null &&
            prop.SetMethod.Visibility().HasFlags(viz));
    }
    
    public B Settable(SetKind kind)
    {
        return Only(kind, static (prop, k) =>
        {
            return k switch
            {
                SetKind.Set => prop.SetMethod is not null,
                SetKind.Init => prop.IsInitOnly(),
                SetKind.Ctor => prop.SetMethod is null,
                _ => throw InvalidEnumException.Create(k),
            };
        });
    }

    public B NotAnIndexer
    {
        get
        {
            return Only(static prop => prop.GetIndexParameters().Length == 0);
        }
    }
    
    public B Indexer(params Type[]? types)
    {
        if (types == null)
            return Only(static property => property.GetIndexParameters().Length == 0);
        
        return Only(types, static (property, ts) =>
            {
                var indexerParameters = property.GetIndexParameters();
                if (indexerParameters.Length != ts.Length)
                    return false;
                for (int i = 0; i < indexerParameters.Length; i++)
                {
                    if (indexerParameters[i].ParameterType != ts[i])
                        return false;
                }

                return true;
            });
    }
    
    public B Indexer(Type[]? types, TypeMatch match)
    {
        if (types == null)
            return Only(static property => property.GetIndexParameters().Length == 0);
        
        return Only(types, match, static (property,t,m) =>
        {
            var indexerParameters = property.GetIndexParameters();
            if (indexerParameters.Length != t.Length)
                return false;
            for (int i = 0; i < indexerParameters.Length; i++)
            {
                if (indexerParameters[i].ParameterType.Matches(t[i], m))
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