namespace ScrubJay.Reflection.Searching.Predication.Members;

public sealed record class TypeMatchCriteria : MemberMatchCriteria<Type>
{
    public ICriteria<Type[]>? GenericTypes { get; set; }

    public override bool Matches([NotNullWhen(true)] Type? type)
    {
        if (!base.Matches(type))
            return false;

        if (!Matches(GenericTypes, type.GenericTypeArguments))
            return false;

        return true;
    }
}