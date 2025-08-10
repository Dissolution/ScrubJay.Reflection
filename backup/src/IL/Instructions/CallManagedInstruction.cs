namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class CallManagedInstruction : ILGeneratorInstruction
{
    public CallingConventions CallingConventions { get; }
    public Type? ReturnType { get; }
    public Type[]? ParameterTypes { get; }
    public Type[]? OptionalParameterTypes { get; }

    public CallManagedInstruction(CallingConventions callingConventions,
        Type? returnType,
        Type[]? parameterTypes,
        Type[]? optionalParameterTypes = null)
        : base(ILGeneratorMethod.CallManaged)
    {
        CallingConventions = callingConventions;
        ReturnType = returnType;
        ParameterTypes = parameterTypes;
        OptionalParameterTypes = optionalParameterTypes;
    }

    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder
            .Render(CallingConventions)
            .Append(", ")
            .Render(ReturnType)
            .Append(", ")
            .Render(ParameterTypes)
            .Append(", ")
            .Render(OptionalParameterTypes);
    }
}