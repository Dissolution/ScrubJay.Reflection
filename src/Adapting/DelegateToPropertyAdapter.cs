namespace ScrubJay.Reflection.Adapting;

[PublicAPI]
public abstract class DelegateToPropertyAdapter : DelegateToMemberAdapter,
    IDelegateToMemberAdapter<PropertyInfo>
{
    public static Result<D> TryAdapt<D>(PropertyInfo property)
        where D : Delegate
    {
        if (property is null)
            return new ArgumentNullException(nameof(property));

        DelegateInfo delInfo = DelegateInfo.New<D>();

        // if our delegate has a return value, then we are a getter
        if (!delInfo.ReturnType.IsVoidLike())
        {
            // find the get method
            var getMethod = property.GetMethod;
            if (getMethod is not null)
            {
                return DelegateToMethodAdapter.TryAdapt<D>(getMethod);
            }
        }
        else
        {
            // find the set method
            var setMethod = property.SetMethod;
            if (setMethod is not null)
            {
                return DelegateToMethodAdapter.TryAdapt<D>(setMethod);
            }
        }
        
        // try to use the backing field?
        var backingField = property.GetBackingField();
        if (backingField is not null)
        {
            return DelegateToFieldAdapter.TryAdapt<D>(backingField);
        }

        return GetError(property, delInfo);
    }

    private DelegateToPropertyAdapter() { }
}