using ScrubJay.Extensions;
using ScrubJay.Reflection.Utilities;

namespace ScrubJay.Reflection.Info;

public record class DelegateSignature
{
    public required Type[] ParameterTypes { get; init; } = Type.EmptyTypes;
    public required Type ReturnType { get; init; } = typeof(void);
}