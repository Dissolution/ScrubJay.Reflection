namespace ScrubJay.Reflection.Searching.Predication.Members;

public sealed record class FieldMatchCriteria : MemberMatchCriteria<FieldInfo>
{
    public ICriteria<Type>? ValueType { get; set; }
    public FieldModifiers? Modifiers { get; set; }
    
    public override bool Matches(FieldInfo? field)
    {
        if (!base.Matches(field))
            return false;

        if (!Matches(ValueType, field.FieldType))
            return false;
        
        if (!HasAnyFlags(Modifiers, field.Modifiers))
            return false;

        return true;
    }
}