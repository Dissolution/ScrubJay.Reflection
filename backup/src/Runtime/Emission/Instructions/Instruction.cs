#pragma warning disable CA1819

namespace ScrubJay.Reflection.Runtime.Emission.Instructions;

public abstract record class Instruction
{
    public int? Offset { get; internal set; }

    /// <summary>
    /// Gets the total number of bytes in the <see cref="InstructionStream"/> this <see cref="Instruction"/> occupies
    /// </summary>
    public abstract int Size { get; }

    protected Instruction() {  }
    protected Instruction(int offset)
    {
        Offset = offset;
    }

    protected internal TextBuilder AppendOffset(TextBuilder text)
    {
        return text.Append("IL_")
            .If(Offset.HasValue,
                tb => tb.Append(Offset, "X4"),
                static tb => tb.Append("????"));
    }

    public override string ToString()
    {
        return TextBuilder.New
            .Invoke(tb => AppendOffset(tb))
            .Append(": ")
            .AppendType(GetType())
            .ToStringAndDispose();
    }
}

