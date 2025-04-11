namespace ScrubJay.Reflection.IL.Instructions;

public abstract class Instruction : IEquatable<Instruction>
{
    public int Offset { get; internal set; } = -1;
    
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

    public virtual void RenderTo(TextBuilder builder)
    {
        builder.Append("IL_")
            .If(Offset < 0, static tb => tb.Append("????"),
                tb => tb.Append(Offset, "X4"))
            .Append(": ");
    }

    public override sealed string ToString() => TextBuilder.New.Invoke(RenderTo).ToStringAndDispose();
}