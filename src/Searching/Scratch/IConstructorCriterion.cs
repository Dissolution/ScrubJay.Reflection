namespace ScrubJay.Reflection.Searching.Scratch;

public interface IConstructorCriterion : IMethodBaseCriterion<ConstructorInfo>
{
    ICriterion<Type>? Type { get; set; }
}

public record class ConstructorCriterion : MethodBaseCriterion<ConstructorInfo>, IConstructorCriterion
{
    public ICriterion<Type>? Type { get; set; }

    public override bool Matches(ConstructorInfo? ctor)
    {
        if (!base.Matches(ctor))
            return false;

        if (Type is not null && !Type.Matches(ctor.DeclaringType))
            return false;

        return true;
    }
}