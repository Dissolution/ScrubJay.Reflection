namespace ScrubJay.Reflection.Searching.Criteria;

public interface IHasAttributesCriterion : ICriterion
{
    ICriterion<Attribute[]> Attributes { get; set; }
}

public interface IHasAttributesCriterionBuilder<out TBuilder>
    where TBuilder : IHasAttributesCriterionBuilder<TBuilder>
{
    TBuilder HasAttribute(Type attributeType);
    TBuilder HasAttribute<TAttribute>() where TAttribute : Attribute;
}

