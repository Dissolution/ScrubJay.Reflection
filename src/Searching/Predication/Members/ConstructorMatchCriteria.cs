namespace ScrubJay.Reflection.Searching.Predication.Members;

public sealed record class ConstructorMatchCriteria : MethodBaseMatchCriteria<ConstructorInfo>
{
    public ICriteria<Type>? ConstructedType { get; set; } 
    
    public override bool Matches([NotNullWhen(true)] ConstructorInfo? ctor)
    {
        if (!base.Matches(ctor))
            return false;

        if (!Matches(ConstructedType, ctor.DeclaringType!))
            return false;

        return true;
    }
}