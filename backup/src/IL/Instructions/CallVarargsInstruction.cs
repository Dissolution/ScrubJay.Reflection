namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public sealed class CallVarargsInstruction : ILGeneratorInstruction
{
    public OpCode OpCode { get; }
    public MethodInfo Method { get; }
    public Type[]? OptionalParameterTypes { get; }

    public CallVarargsInstruction(OpCode opCode, MethodInfo method, Type[]? optionalParameterTypes = null) 
        : base(ILGeneratorMethod.CallVarargs)
    {
        OpCode = opCode;
        Method = method;
        OptionalParameterTypes = optionalParameterTypes;        
    }

    internal protected override TextBuilder RenderArgs(TextBuilder builder)
    {
        return builder
            .Render(OpCode)
            .Append(", ")
            .Render(Method)
            .Append(", ")
            .Render(OptionalParameterTypes);
    }
}