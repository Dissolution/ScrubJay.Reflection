using ScrubJay.Reflection.Extensions;

namespace ScrubJay.Reflection.Info;

public record class MethodSignature : DelegateSignature
{
    public static MethodSignature For(MethodBase method)
    {
        return new MethodSignature
        {
            Name = method.Name,
            ParameterTypes = method.GetParameterTypes(),
            ReturnType = method.ReturnType(),
        };
    }

    public required string Name { get; init; } = "Invoke";
}