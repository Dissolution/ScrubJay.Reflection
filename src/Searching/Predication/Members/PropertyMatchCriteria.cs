namespace ScrubJay.Reflection.Searching.Predication.Members;

public record class PropertyMatchCriteria : MemberMatchCriteria<PropertyInfo>
{
    public ICriteria<Type>? ValueType { get; set; }
    public ICriteria<MethodInfo?>? Getter { get; set; }
    public ICriteria<MethodInfo?>? Setter { get; set; }

    public override bool Matches([NotNullWhen(true)] PropertyInfo? property)
    {
        if (!base.Matches(property))
            return false;

        if (!Matches(ValueType, property.PropertyType))
            return false;

        if (!Matches(Getter, property.GetMethod))
            return false;

        if (!Matches(Setter, property.SetMethod))
            return false;
        
        return true;
    }
}