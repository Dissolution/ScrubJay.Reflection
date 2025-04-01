namespace ScrubJay.Sigil;

/// <summary>
/// Represents an ExceptionBlock, which is roughly analogous to a try + catch + finally block in C#.
///
/// To create an ExceptionBlock call BeginExceptionBlock().
/// </summary>
public class ExceptionBlock : IOwned
{
    object IOwned.Owner { get { return ((IOwned)SigilLabel).Owner; } }

    /// <summary>
    /// A label which marks the end of the ExceptionBlock.
    ///
    /// This Label is meant to be targetted by Leave() from anywhere except a FinallyBlock
    /// in the ExceptionBlock.
    ///
    /// Remember that it is illegal to branch from within an ExceptionBlock to outside.
    /// </summary>
    public SigilLabel SigilLabel { get; private set; }

    internal ExceptionBlock(SigilLabel sigilLabel)
    {
        SigilLabel = sigilLabel;
    }
}
