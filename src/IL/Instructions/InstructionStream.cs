namespace ScrubJay.Reflection.IL.Instructions;

public sealed class InstructionStream : IRenderable
{
    private readonly List<(ILOffset Offset, Instruction Instruction)> _instructions = [];
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
            int size = _instructions.Sum(static i => i.Instruction.Size);
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
        var offset = _ilOffset;
        _ilOffset = offset + instruction.Size;
        _instructions.Add((offset, instruction));
    }

    public Option<Instruction> FindByOffset(ILOffset offset)
    {
        if (offset < 0 || offset >= _ilOffset)
            return None();
        foreach (var tuple in _instructions)
        {
            if (tuple.Offset == offset)
                return Some(tuple.Instruction);
            if (tuple.Offset > offset)
                break;
        }

        return None();
    }

    public bool Contains(Instruction item)
    {
        throw new NotImplementedException();
    }

    public void RenderTo(TextBuilder builder)
    {
        builder.EnumerateAndLineDelimit(_instructions, write);
        return;

        static void write(TextBuilder builder, (ILOffset Offset, Instruction Instruction) tuple)
        {
            var (offset, instruction) = tuple;
            if (instruction is OpCodeInstruction oci)
            {
                builder
                    .Render(offset)
                    .Append(": ")
                    .Align(oci.OpCode.Name!, 13, alignment: Alignment.Right)
                    .Invoke(oci.RenderArgs);
            }
            else if (instruction is ILGeneratorInstruction ilgi)
            {
                builder
                    .Align(ilgi.ILGenMethod.Render(), 22, alignment: Alignment.Right)
                    .Append('(')
                    .Invoke(ilgi.RenderArgs)
                    .Append(')');
            }
            else
            {
                throw new UnreachableException();
            }
        }
    }
}