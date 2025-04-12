namespace ScrubJay.Reflection.IL.Instructions;

[PublicAPI]
public abstract class Instruction : IEquatable<Instruction>, IRenderable
{
    public ILOffset Offset { get; internal set; } = ILOffset.Unknown;
    
    public abstract int Size { get; }

    protected internal Instruction() { }

    public bool Equals(Instruction? instruction)
    {
        if (instruction is null)
            return false;
        if (ReferenceEquals(this, instruction))
            return true;
        return instruction.Offset == this.Offset &&
            instruction.Size == this.Size;
    }

    public override bool Equals(object? obj)
        => obj is Instruction instr && Equals(instr);

    public override sealed int GetHashCode()
        => Throw.NotSupported<int>($"An {GetType().NameOf()} should only be stored in an {typeof(InstructionStream).NameOf()}");

    public virtual void RenderTo<B>(B builder) 
        where B : TextBuilderBase<B>
    {
        builder.Invoke(Offset.RenderTo!)
            .Append(": ");
    }

    public override sealed string ToString() => TextBuilder.Build(RenderTo);
}