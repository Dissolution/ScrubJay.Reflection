using ScrubJay.Text;

namespace ScrubJay.Reflection.Runtime.Emission;

public class InstructionStream : LinkedList<InstructionLine>
{
    public InstructionLine? FindByOffset(int offset)
    {
        if (offset < 0 || Count == 0)
            return null;
        foreach (var streamLine in this)
        {
            if (streamLine.Offset.TryGetValue(out var lineOffset))
            {
                if (lineOffset == offset) return streamLine;
                if (lineOffset > offset) return null;
            }
        }
        return null;
    }

    public void RemoveAfter(LinkedListNode<InstructionLine>? node)
    {
        if (node is null) return;
        var last = Last;
        if (last is not null && last.Value != node.Value)
        {
            RemoveLast();
        }
    }
    
    public override string ToString()
    {
        using var _ = StringBuilderPool.Borrow(out var sb);
        foreach (var line in this)
        {
            sb.Append(line).AppendLine();
        }
        return sb.ToString();
    }
}