using ScrubJay.Collections;

namespace ScrubJay.Reflection.Searching.Scratch;

public interface IParameterCriterion : ICriterion<ParameterInfo>,
    IAttributesCriterion
{
    ICriterion<string>? Name { get; set; }
    ICriterion<Type>? Type { get; set; }
    RefKind RefKind { get; set; }
    ICriterion<object?>? Default { get; set; }
}

public record class ParameterCriterion : IParameterCriterion
{
    public TypeSet? RequiredAttributes { get; set; } = null;
    public ICriterion<string>? Name { get; set; } = null;
    public ICriterion<Type>? Type { get; set; } = null;
    public RefKind RefKind { get; set; } = RefKind.Any;
    public ICriterion<object?>? Default { get; set; } = null;

    public bool Matches(ParameterInfo? parameter)
    {
        if (parameter is null) 
            return false;
        
        if (Name is not null && !Name.Matches(parameter.Name))
            return false;
        
        var paramRefType = parameter.RefKind(out var parameterType);
        if (!RefKind.HasAnyFlags(paramRefType))
            return false;
        
        if (Type is not null && !Type.Matches(parameterType))
            return false;

        if (Default is not null)
        {
            if (!parameter.HasDefaultValue)
                return false;
            if (!Default.Matches(parameter.DefaultValue))
                return false;
        }
        
        
        if (RequiredAttributes is not null && RequiredAttributes.Count > 0)
        {
            var attributes = Attribute.GetCustomAttributes(parameter);
            if (!RequiredAttributes.All(reqAttrType => attributes.Any(attr => attr.GetType() == reqAttrType)))
                return false;
        }
        
        return true;
    }
}