namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class CallUnmanagedInstruction : ILGeneratorInstruction
{
    public CallingConvention CallingConvention { get; }
    public Type? ReturnType { get; }
    public Type[]? ParameterTypes { get; }

    public CallUnmanagedInstruction(
        CallingConvention callingConvention, 
        Type? returnType, 
        Type[]? parameterTypes) 
        : base(ILGeneratorMethod.CallUnmanaged)
    {
        CallingConvention = callingConvention;
        ReturnType = returnType;
        ParameterTypes = parameterTypes;
    }

    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder
            .Render(CallingConvention)
            .Append(", ")
            .Render(ReturnType)
            .Append(", ")
            .Render(ParameterTypes);
    }
}