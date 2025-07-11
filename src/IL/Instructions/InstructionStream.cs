using ScrubJay.Reflection.IL.LabelOffSetManagement;

namespace ScrubJay.Reflection.IL.Instructions;

public interface IInstructions : IReadOnlyCollection<Instruction>
{
    /// <summary>
    /// Gets the total size of all <see cref="Instruction">Instructions</see>, in <see cref="byte">bytes</see>
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Tries to find the <see cref="Instruction"/> with the given <see cref="ILOffset"/>
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    Option<Instruction> FindByOffset(ILOffset offset);
}

public interface IInstructionStream : IInstructions
{
    /// <summary>
    /// Adds a new <see cref="Instruction"/> at the end of this instruction stream
    /// </summary>
    /// <param name="instruction"></param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="instruction"/> is <c>null</c>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the <see cref="Instruction"/> has an invalid <see cref="Instruction.Offset"/>
    /// </exception>
    void Add(Instruction instruction);
}

public sealed class InstructionStream : IInstructionStream
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

    public Option<Instruction> FindByOffset(ILOffset offset)
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
            .EnumerateAndLineDelimit(_instructions, static (tb, instr) => instr.RenderTo(tb))
            .ToStringAndDispose();
}