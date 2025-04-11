namespace ScrubJay.Reflection.Extensions;

/// <summary>
/// Extensions related to <see cref="System.Reflection.NullabilityInfo"/>
/// </summary>
public static class NullabilityInfoExtensions
{
    private static readonly NullabilityInfoContext _nullabilityInfoContext = new();
    
    public static NullabilityInfo? NullabilityInfo(this ParameterInfo? parameter)
    {
        if (parameter is null)
            return null;

        try
        {
            return _nullabilityInfoContext.Create(parameter);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    [return: NotNullIfNotNull(nameof(field))]
    public static NullabilityInfo? NullabilityInfo(this FieldInfo? field)
    {
        if (field is null)
            return null;
        return _nullabilityInfoContext.Create(field);
    }
    
    [return: NotNullIfNotNull(nameof(property))]
    public static NullabilityInfo? NullabilityInfo(this PropertyInfo? property)
    {
        if (property is null)
            return null;
        return _nullabilityInfoContext.Create(property);
    }
    
    [return: NotNullIfNotNull(nameof(@event))]
    public static NullabilityInfo? NullabilityInfo(this EventInfo? @event)
    {
        if (@event is null)
            return null;
        return _nullabilityInfoContext.Create(@event);
    }
    
    public static NullabilityInfo? NullabilityInfo(this MemberInfo? member)
    {
        return member switch
        {
            FieldInfo field => _nullabilityInfoContext.Create(field),
            PropertyInfo property => _nullabilityInfoContext.Create(property),
            EventInfo @event => _nullabilityInfoContext.Create(@event),
            _ => null,
        };
    }
}