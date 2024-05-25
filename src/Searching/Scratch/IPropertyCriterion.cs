namespace ScrubJay.Reflection.Searching.Scratch;

public interface IPropertyCriterion : IMemberCriterion<PropertyInfo>
{
    ICriterion<Type>? Type { get; set; }
    ICriterion<MethodInfo>? Getter { get; set; }
    ICriterion<MethodInfo>? Setter { get; set; }
}

public record class PropertyCriterion : MemberCriterion<PropertyInfo>, IPropertyCriterion
{
    public ICriterion<Type>? Type { get; set; }
    public ICriterion<MethodInfo>? Getter { get; set; }
    public ICriterion<MethodInfo>? Setter { get; set; }

    public override bool Matches([NotNullWhen(true)] PropertyInfo? property)
    {
        if (!base.Matches(property))
            return false;

        if (Type is not null && !Type.Matches(property.PropertyType))
            return false;

        if (Getter is not null)
        {
            var getMethod = property.GetMethod;
            if (getMethod is null)
                return false;
            if (!Getter.Matches(getMethod))
                return false;
        }

        if (Setter is not null)
        {
            var setMethod = property.SetMethod;
            if (setMethod is null) 
                return false;
            if (!Setter.Matches(setMethod)) 
                return false;
        }

        return true;
    }
}