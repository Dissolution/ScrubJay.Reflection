namespace ScrubJay.Reflection.Searching.Scratch;

public interface IFieldCriterion : IMemberCriterion<FieldInfo>
{
    ICriterion<Type>? Type { get; set; }
    FieldModifiers Modifiers { get; set; }
}

public record class FieldCriterion : MemberCriterion<FieldInfo>, IFieldCriterion
{
    public ICriterion<Type>? Type { get; set; }
    public FieldModifiers Modifiers { get; set; }

    public override bool Matches(FieldInfo? field)
    {
        if (!base.Matches(field))
            return false;

        if (Type is not null && !Type.Matches(field.FieldType))
            return false;
        if (!Modifiers.HasAnyFlags(field.Modifiers()))
            return false;

        return true;
    }
}