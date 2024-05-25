using ScrubJay.Collections;

namespace ScrubJay.Reflection.Searching.Scratch;

public interface IAttributesCriterion : ICriterion
{
    TypeSet? RequiredAttributes { get; set; }
}