namespace ScrubJay.Sigil.Emission.NonGeneric;

public partial class Emit
{
    /// <summary>
    /// Start a new exception block.  This is roughly analogous to a `try` block in C#, but an exception block contains it's catch and finally blocks.
    /// </summary>
    public ExceptionBlock BeginExceptionBlock()
    {
        return _innerEmit.BeginExceptionBlock();
    }

    /// <summary>
    /// Start a new exception block.  This is roughly analogous to a `try` block in C#, but an exception block contains it's catch and finally blocks.
    /// </summary>
    public Emit BeginExceptionBlock(out ExceptionBlock forTry)
    {
        _innerEmit.BeginExceptionBlock(out forTry);
        return this;
    }

    /// <summary>
    /// Ends the given exception block.
    ///
    /// All catch and finally blocks associated with the given exception block must be ended before this method is called.
    /// </summary>
    public Emit EndExceptionBlock(ExceptionBlock forTry)
    {
        _innerEmit.EndExceptionBlock(forTry);
        return this;
    }

    /// <summary>
    /// Begins a catch block for the given exception type in the given exception block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public CatchBlock BeginCatchBlock<TExceptionType>(ExceptionBlock forTry)
    {
        return _innerEmit.BeginCatchBlock<TExceptionType>(forTry);
    }

    /// <summary>
    /// Begins a catch block for the given exception type in the given exception block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public Emit BeginCatchBlock<TExceptionType>(ExceptionBlock forTry, out CatchBlock forCatch)
    {
        _innerEmit.BeginCatchBlock<TExceptionType>(forTry, out forCatch);
        return this;
    }

    /// <summary>
    /// Begins a catch block for all exceptions in the given exception block
    ///
    /// The given exception block must still be open.
    ///
    /// Equivalent to BeginCatchBlock(typeof(Exception), forTry).
    /// </summary>
    public CatchBlock BeginCatchAllBlock(ExceptionBlock forTry)
    {
        return _innerEmit.BeginCatchAllBlock(forTry);
    }

    /// <summary>
    /// Begins a catch block for all exceptions in the given exception block
    ///
    /// The given exception block must still be open.
    ///
    /// Equivalent to BeginCatchBlock(typeof(Exception), forTry).
    /// </summary>
    public Emit BeginCatchAllBlock(ExceptionBlock forTry, out CatchBlock forCatch)
    {
        _innerEmit.BeginCatchAllBlock(forTry, out forCatch);
        return this;
    }

    /// <summary>
    /// Begins a catch block for the given exception type in the given exception block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public CatchBlock BeginCatchBlock(ExceptionBlock forTry, Type exceptionType)
    {
        return _innerEmit.BeginCatchBlock(forTry, exceptionType);
    }

    /// <summary>
    /// Begins a catch block for the given exception type in the given exception block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public Emit BeginCatchBlock(ExceptionBlock forTry, Type exceptionType, out CatchBlock forCatch)
    {
        _innerEmit.BeginCatchBlock(forTry, exceptionType, out forCatch);
        return this;
    }

    /// <summary>
    /// Ends the given catch block.
    /// </summary>
    public Emit EndCatchBlock(CatchBlock forCatch)
    {
        _innerEmit.EndCatchBlock(forCatch);
        return this;
    }

    /// <summary>
    /// Begins a finally block on the given exception block.
    ///
    /// Only one finally block can be defined per exception block, and the block cannot appear within a catch block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public Emit BeginFinallyBlock(ExceptionBlock forTry, out FinallyBlock forFinally)
    {
        _innerEmit.BeginFinallyBlock(forTry, out forFinally);
        return this;
    }

    /// <summary>
    /// Begins a finally block on the given exception block.
    ///
    /// Only one finally block can be defined per exception block, and the block cannot appear within a catch block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public FinallyBlock BeginFinallyBlock(ExceptionBlock forTry)
    {
        return _innerEmit.BeginFinallyBlock(forTry);
    }

    /// <summary>
    /// Ends the given finally block.
    /// </summary>
    public Emit EndFinallyBlock(FinallyBlock forFinally)
    {
        _innerEmit.EndFinallyBlock(forFinally);
        return this;
    }
}
