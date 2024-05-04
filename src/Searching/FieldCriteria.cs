namespace ScrubJay.Reflection.Searching;

public record class FieldCriteria : MemberCriteria
{
    public Type? FieldType { get; set; } = null;
    public TypeCriteria TypeCriteria { get; set; } = TypeCriteria.Any;
    public FieldModifiers Modifiers { get; set; }
}