using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Decompilation;

public sealed class InstructionStream : 
    IReadOnlyCollection<Instruction>,
    IEnumerable<Instruction>,
    IRenderable
{
    private readonly List<Instruction> _instructions = [];
    private int _size = 0;
    
    
    public int Count => _instructions.Count;

    public int Size => _size;


    public void Add(Instruction instruction)
    {
        ILOffset offset = _size;
        if (instruction.Offset.IsUnknown)
        {
            instruction = instruction with { Offset = offset };
        }
        else
        {
            if (instruction.Offset != offset)
                throw new ArgumentException(null, nameof(instruction));
        }
        _instructions.Add(instruction);
        _size += instruction.Size;
    }
    
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<Instruction> GetEnumerator()
    {
        return _instructions.GetEnumerator();
    }

    public TextBuilder RenderTo(TextBuilder builder)
    {
        return builder.Delimit(Delimiter.NewLine, _instructions, static (tb, i) => tb.Render(i));
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}