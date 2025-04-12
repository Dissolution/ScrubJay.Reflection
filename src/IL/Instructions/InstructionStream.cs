namespace ScrubJay.Reflection.IL.Instructions;

public sealed class InstructionStream : IReadOnlyCollection<Instruction>
{
    private readonly List<Instruction> _instructions = [];
    private int _ilOffset = 0;

    public int Count => _instructions.Count;

    /// <summary>
    /// Gets the total size of all instructions, in bytes
    /// </summary>
    public int Size
    {
        get
        {
#if DEBUG
            int size = _instructions.Sum(static i => i.Size);
            Debug.Assert(size == _ilOffset);
            return size;
#else
            return _ilOffset;
#endif
        }
    }

    public void Add(Instruction instruction)
    {
        Throw.IfNull(instruction);
        if (instruction.Offset == ILOffset.Unknown)
        {
            instruction.Offset = _ilOffset;
        }
        else
        {
            Debug.Assert(instruction.Offset == _ilOffset);
        }

        _ilOffset += instruction.Size;
        _instructions.Add(instruction);
    }

    public Option<Instruction> TryFindByOffset(int offset)
    {
        if (offset < 0 || offset >= _ilOffset)
            return None();
        foreach (var instr in _instructions)
        {
            if (instr.Offset == offset)
                return Some(instr);
            if (instr.Offset > offset)
                break;
        }
        return None();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public IEnumerator<Instruction> GetEnumerator() => _instructions.GetEnumerator();

    public override string ToString()
        => TextBuilder.New
            .Delimit(
                static tb => tb.NewLine(), 
                _instructions,
                static (tb, instr) => instr.RenderTo(tb))
            .ToStringAndDispose();
}