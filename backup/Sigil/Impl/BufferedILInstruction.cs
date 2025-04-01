namespace ScrubJay.Sigil.Impl;

internal class BufferedILInstruction
{
    public bool DefinesLabel { get; internal set; }
    public Sigil.SigilLabel MarksSigilLabel { get; internal set; }

    public bool StartsExceptionBlock { get; internal set; }
    public bool EndsExceptionBlock { get; internal set; }

    public bool StartsCatchBlock { get; internal set; }
    public bool EndsCatchBlock { get; internal set; }

    public bool StartsFinallyBlock { get; internal set; }
    public bool EndsFinallyBlock { get; internal set; }

    public bool DeclaresLocal { get; internal set; }

    public OpCode? IsInstruction { get; internal set; }

    public Type MethodReturnType { get; internal set; }
    public IEnumerable<Type> MethodParameterTypes { get; internal set; }
    // TODO: see https://github.com/dotnet/corefx/issues/4543 item 4
#if !NETSTANDARD
    public bool TakesTypedReference()
    {
        var instr = this;

        if (instr.MethodReturnType == typeof(TypedReference)) return true;

        return instr.MethodParameterTypes.Any(p => p == typeof(TypedReference));
    }
#endif

    public bool TakesManagedPointer()
    {
        var instr = this;

        if (instr.MethodReturnType.IsPointer) return true;

        return instr.MethodParameterTypes.Any(p => p.IsPointer);
    }

    internal bool TakesByRefArgs()
    {
        var instr = this;

        return instr.MethodParameterTypes.Any(p => p.IsByRef);
    }
}
