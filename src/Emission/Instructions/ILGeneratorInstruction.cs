//namespace ScrubJay.Reflection.Emission.Instructions;
//
//public enum ILGeneratorMethod
//{
//    BeginCatchBlock,
//    BeginExceptFilterBlock,
//    BeginExceptionBlock,
//    EndExceptionBlock,
//    BeginFaultBlock,
//    BeginFinallyBlock,
//    ThrowException,
//    UsingNamespace,
//    BeginScope,
//    EndScope,
//    CallManaged,
//    CallUnmanaged,
//    CallVarargs,
//    DeclareLocal,
//    DefineLabel,
//    MarkLabel,
//    WriteLine,
//}
//
//
//public sealed record class ILGeneratorInstruction : Instruction
//{
//    public required ILGeneratorMethod ILGenMethod { get; init; }
//
//    public Values<object?> Parameters { get; set; } = [];
//
//    public override int Size => 0;
//
//    public ILGeneratorInstruction() { }
//
//    [SetsRequiredMembers]
//    public ILGeneratorInstruction(ILGeneratorMethod ilGenMethod)
//    {
//        ILGenMethod = ilGenMethod;
//    }
//
//    [SetsRequiredMembers]
//    public ILGeneratorInstruction(ILGeneratorMethod ilGenMethod, object? arg)
//    {
//        ILGenMethod = ilGenMethod;
//        Parameters = Values<object?>.Create(arg);
//    }
//
//    [SetsRequiredMembers]
//    public ILGeneratorInstruction(ILGeneratorMethod ilGenMethod, params object?[] args)
//    {
//        ILGenMethod = ilGenMethod;
//        Parameters = Values<object?>.Create(args);
//    }
//
//    private void AppendTo(TextBuilder text)
//    {
//        text.Append(ILGenMethod);
//        Parameters.Match(
//            () => {  },
//            arg => text.Append(' ').Append(arg),
//            args => text.Append('[').DelimitAppend(", ", args).Append(']'));
//    }
//
//    public override string ToString() => TextBuilder.New
//        .Invoke(AppendTo)
//        .ToStringAndDispose();
//}
