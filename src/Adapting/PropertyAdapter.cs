namespace ScrubJay.Reflection.Adapting;


public class PropertyGetterAdapter<I, T> :
    MemberDelegateAdapter<PropertyGetterAdapter<I, T>, PropertyInfo, Getter<I, T>>
{
    public override Result<Getter<I, T>> TryAdapt(PropertyInfo? property)
    {
        if (property is null)
            return GetEx(property);
        
        // find the get method
        var getMethod = property.GetMethod;
        if (getMethod is not null)
        {
            return MethodDelegateAdapter<Getter<I, T>>.Instance.TryAdapt(getMethod);
        }
        
        // try to use the backing field?
        var backingField = property.GetBackingField();
        if (backingField is not null)
        {
            return FieldGetterAdapter<I, T>.Instance.TryAdapt(backingField);
        }

        return GetEx(property);
    }
}

public class IndexerPropertyGetterAdapter<D> :
    MemberDelegateAdapter<IndexerPropertyGetterAdapter<D>, PropertyInfo, D>
    where D : Delegate
{
    public override Result<D> TryAdapt(PropertyInfo? property)
    {
        if (property is null)
            return GetEx(property);
        
        var indexParameters = property.GetIndexParameters();
        var getMethod = property.GetMethod;
        var getMethodParameters = getMethod.Parameters();
        Debugger.Break();

        return MethodDelegateAdapter<D>.Instance.TryAdapt(getMethod!);
    }
}

public class PropertySetterAdapter<I, T> :
    MemberDelegateAdapter<PropertySetterAdapter<I, T>, PropertyInfo, Setter<I, T>>
{
    public override Result<Setter<I, T>> TryAdapt(PropertyInfo? property)
    {
        if (property is null)
            return GetEx(property);
        
        // find the set method
        var setMethod = property.SetMethod;
        if (setMethod is not null)
        {
            return MethodDelegateAdapter<Setter<I, T>>.Instance.TryAdapt(setMethod);
        }
        
        // try to use the backing field?
        var backingField = property.GetBackingField();
        if (backingField is not null)
        {
            return FieldSetterAdapter<I, T>.Instance.TryAdapt(backingField);
        }

        return GetEx(property);
    }
}

public class IndexerPropertySetterAdapter<D> :
    MemberDelegateAdapter<IndexerPropertySetterAdapter<D>, PropertyInfo, D>
    where D : Delegate
{
    public override Result<D> TryAdapt(PropertyInfo? property)
    {
        if (property is null)
            return GetEx(property);
        
        var indexParameters = property.GetIndexParameters();
        var setMethod = property.SetMethod;
        var setMethodParameters = setMethod.Parameters();
        Debugger.Break();

        return MethodDelegateAdapter<D>.Instance.TryAdapt(setMethod!);
    }
}