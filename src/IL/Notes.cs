namespace ScrubJay.Reflection.IL;

public class Notes
{
    static Notes()
    {
       
        OperandType opType = default!;
        switch (opType)
        {
            case OperandType.InlineBrTarget:
                break;
            case OperandType.InlineField:
                break;
            case OperandType.InlineI:
                break;
            case OperandType.InlineI8:
                break;
            case OperandType.InlineMethod:
                break;
            case OperandType.InlineNone:
                break;
            case (OperandType)6:
                break;
            case OperandType.InlineR:
                break;
            case (OperandType)8:
                break;
            case OperandType.InlineSig:
                break;
            case OperandType.InlineString:
                break;
            case OperandType.InlineSwitch:
                break;
            case OperandType.InlineTok:
                break;
            case OperandType.InlineType:
                break;
            case OperandType.InlineVar:
                break;
            case OperandType.ShortInlineBrTarget:
                break;
            case OperandType.ShortInlineI:
                break;
            case OperandType.ShortInlineR:
                break;
            case OperandType.ShortInlineVar:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}