//#pragma warning disable CA1819
//
//namespace ScrubJay.Reflection.Emission.Instructions;
//
//public interface IInstructionStream<TInstruction> : IReadOnlyCollection<TInstruction>
//    where TInstruction : Instruction
//{
//    void Add(TInstruction instruction);
//    Option<TInstruction> TryFindByOffset(int offset);
//}
//
//public class InstructionStream<TInstruction> : IInstructionStream<TInstruction>
//    where TInstruction : Instruction
//{
//    private readonly List<TInstruction> _instructions = [];
//    private int _offset;
//
//    public int Count => _instructions.Count;
//
//    public int Size
//    {
//        get
//        {
//#if DEBUG
//            int size = _instructions.Sum(static i => i.Size);
//            Debug.Assert(size == _offset);
//            return size;
//#else
//            return _offset;
//#endif
//        }
//    }
//
//    public void Add(TInstruction instruction)
//    {
//        Throw.IfNull(instruction);
//        // If it has no offset, we give it one
//        if (instruction.Offset == -1)
//        {
//            instruction.Offset = _offset;
//        }
//        else
//        {
//            // Validate that this is the next instruction
//            if (instruction.Offset != _offset)
//            {
//                throw new ArgumentException(
//                    $"Instruction '{instruction}' does not fit next in this stream",
//                    nameof(instruction));
//            }
//        }
//        _offset += instruction.Size;
//        _instructions.Add(instruction);
//    }
//
//    public Option<TInstruction> TryFindByOffset(int offset)
//    {
//        if (offset < 0 || offset >= _offset)
//            return None();
//        foreach (var instr in _instructions)
//        {
//            if (instr.Offset == offset)
//                return Some(instr);
//            if (instr.Offset > offset)
//                break;
//        }
//        return None();
//    }
//
//    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//    public IEnumerator<TInstruction> GetEnumerator() => _instructions.GetEnumerator();
//
//    public override string ToString()
//    {
//        return TextBuilder.New
//            .DelimitAppend(static t => t.NewLine(), _instructions)
//            .ToStringAndDispose();
//    }
//}
