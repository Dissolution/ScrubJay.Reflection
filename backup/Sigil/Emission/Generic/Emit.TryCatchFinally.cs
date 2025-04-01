namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    private Dictionary<string, ExceptionBlock> _disassembledExceptionBlocks;
    private Dictionary<string, CatchBlock> _disassembledCatchBlocks;
    private Dictionary<string, FinallyBlock> _disassembledFinallyBlocks;

    // A version of BeginExceptionBlock used by the Disassembler to keep track of
    //   exception blocks while re-emitting
    internal void BeginExceptionBlock(string storeUnderName)
    {
        var block = BeginExceptionBlock();

        if (_disassembledExceptionBlocks == null)
        {
            _disassembledExceptionBlocks = new Dictionary<string, ExceptionBlock>();
        }

        _disassembledExceptionBlocks[storeUnderName] = block;
    }

    internal void EndExceptionBlock(string lookupByName)
    {
        var block = _disassembledExceptionBlocks[lookupByName];

        EndExceptionBlock(block);

        _disassembledExceptionBlocks.Remove(lookupByName);
    }

    internal void BeginCatchBlock(string lookupExcName, Type exceptionType, string storeUnderName)
    {
        var block = _disassembledExceptionBlocks[lookupExcName];
        if (_disassembledCatchBlocks == null)
        {
            _disassembledCatchBlocks = new Dictionary<string, CatchBlock>();
        }

        _disassembledCatchBlocks[storeUnderName] = BeginCatchBlock(block, exceptionType);
    }

    internal void EndCatchBlock(string lookupByName)
    {
        var c = _disassembledCatchBlocks[lookupByName];
        EndCatchBlock(c);

        _disassembledCatchBlocks.Remove(lookupByName);
    }

    internal void BeginFinallyBlock(string lookupExcName, string storeUnderName)
    {
        var block = _disassembledExceptionBlocks[lookupExcName];
        if (_disassembledFinallyBlocks == null)
        {
            _disassembledFinallyBlocks = new Dictionary<string, FinallyBlock>();
        }

        _disassembledFinallyBlocks[storeUnderName] = BeginFinallyBlock(block);
    }

    internal void EndFinallyBlock(string lookupByName)
    {
        var f = _disassembledFinallyBlocks[lookupByName];
        EndFinallyBlock(f);

        _disassembledFinallyBlocks.Remove(lookupByName);
    }

    /// <summary>
    /// Start a new exception block.  This is roughly analogous to a `try` block in C#, but an exception block contains it's catch and finally blocks.
    /// </summary>
    public ExceptionBlock BeginExceptionBlock()
    {
        if (_mustMark)
        {
            MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
        }

        UpdateState(Wrap(new[] { new StackTransition(0) }, "BeginExceptionBlock"));

        var labelDel = _il.BeginExceptionBlock();
        var label = new SigilLabel(this, labelDel, AutoNamer.Next(this, "__exceptionBlockEnd"));

        _currentLabels[label.Name] = label;

        var ret = new ExceptionBlock(label);

        _tryBlocks[ret] = Tuple.Create(_il.Index, -1);

        _currentExceptionBlock.Push(ret);

        return ret;
    }

    /// <summary>
    /// Start a new exception block.  This is roughly analogous to a `try` block in C#, but an exception block contains it's catch and finally blocks.
    /// </summary>
    public Emit<TDelegateType> BeginExceptionBlock(out ExceptionBlock forTry)
    {
        forTry = BeginExceptionBlock();

        return this;
    }

    /// <summary>
    /// Ends the given exception block.
    ///
    /// All catch and finally blocks associated with the given exception block must be ended before this method is called.
    /// </summary>
    public Emit<TDelegateType> EndExceptionBlock(ExceptionBlock forTry)
    {
        if (forTry == null)
        {
            throw new ArgumentNullException("forTry");
        }

        if (((IOwned)forTry).Owner != this)
        {
            FailOwnership(forTry);
        }

        var location = _tryBlocks[forTry];

        // Can't close the same exception block twice
        if (location.Item2 != -1)
        {
            throw new InvalidOperationException("ExceptionBlock has already been ended");
        }

        if (_currentExceptionBlock.Count > 0 && forTry != _currentExceptionBlock.Peek())
        {
            throw new InvalidOperationException("Cannot end outer ExceptionBlock " + forTry + " while inner EmitExceptionBlock " + _currentExceptionBlock.Peek() + " is open");
        }

        // Can't close an exception block while there are outstanding catch blocks
        foreach (var kv in _catchBlocks)
        {
            if (kv.Key.ExceptionBlock != forTry) continue;

            if (kv.Value.Item2 == -1)
            {
                throw new InvalidOperationException("Cannot end ExceptionBlock, CatchBlock " + kv.Key + " has not been ended");
            }
        }

        foreach (var kv in _finallyBlocks)
        {
            if (kv.Key.ExceptionBlock != forTry) continue;

            if (kv.Value.Item2 == -1)
            {
                throw new InvalidOperationException("Cannot end ExceptionBlock, FinallyBlock " + kv.Key + " has not been ended");
            }
        }

        if (!_catchBlocks.Any(k => k.Key.ExceptionBlock == forTry) && !_finallyBlocks.Any(k => k.Key.ExceptionBlock == forTry))
        {
            throw new InvalidOperationException("Cannot end ExceptionBlock without defining at least one of a catch or finally block");
        }

        _il.EndExceptionBlock();

        _tryBlocks[forTry] = Tuple.Create(location.Item1, _il.Index);

        _marks[forTry.SigilLabel] = _il.Index;

        _currentExceptionBlock.Pop();

        if (_mustMark)
        {
            MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
        }

        return this;
    }

    /// <summary>
    /// Begins a catch block for the given exception type in the given exception block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public CatchBlock BeginCatchBlock<TExceptionType>(ExceptionBlock forTry)
    {
        return BeginCatchBlock(forTry, typeof(TExceptionType));
    }

    /// <summary>
    /// Begins a catch block for the given exception type in the given exception block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public Emit<TDelegateType> BeginCatchBlock<TExceptionType>(ExceptionBlock forTry, out CatchBlock forCatch)
    {
        forCatch = BeginCatchBlock<TExceptionType>(forTry);

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
        return BeginCatchBlock<Exception>(forTry);
    }

    /// <summary>
    /// Begins a catch block for all exceptions in the given exception block
    ///
    /// The given exception block must still be open.
    ///
    /// Equivalent to BeginCatchBlock(typeof(Exception), forTry).
    /// </summary>
    public Emit<TDelegateType> BeginCatchAllBlock(ExceptionBlock forTry, out CatchBlock forCatch)
    {
        forCatch = BeginCatchAllBlock(forTry);

        return this;
    }

    /// <summary>
    /// Begins a catch block for the given exception type in the given exception block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public CatchBlock BeginCatchBlock(ExceptionBlock forTry, Type exceptionType)
    {
        if (exceptionType == null)
        {
            throw new ArgumentNullException("exceptionType");
        }

        if (forTry == null)
        {
            throw new ArgumentNullException("forTry");
        }

        if (((IOwned)forTry).Owner != this)
        {
            FailOwnership(forTry);
        }

        if (_currentExceptionBlock.Count > 0 && forTry != _currentExceptionBlock.Peek())
        {
            throw new InvalidOperationException("Cannot start CatchBlock on " + forTry + " while inner ExceptionBlock is still open");
        }

        if (!typeof(Exception).IsAssignableFrom(exceptionType))
        {
            throw new ArgumentException("BeginCatchBlock expects a type descending from Exception, found " + exceptionType, "exceptionType");
        }

        var currentlyOpen = _catchBlocks.Where(c => c.Key.ExceptionBlock == forTry && c.Value.Item2 == -1).Select(s => s.Key).SingleOrDefault();
        if (currentlyOpen != null)
        {
            throw new InvalidOperationException("Cannot start a new catch block, " + currentlyOpen + " has not been ended");
        }

        if (_mustMark)
        {
            MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
        }

        UpdateState(Wrap(new[] { new StackTransition(0) }, "BeginCatchBlock"));

        var tryBlock = _tryBlocks[forTry];

        if (tryBlock.Item2 != -1)
        {
            throw new SigilVerificationException("BeginCatchBlock expects an unclosed exception block, but " + forTry + " is already closed", _il.Instructions(_allLocals));
        }

        _il.BeginCatchBlock(exceptionType);

        UpdateState(Wrap(StackTransition.Push(exceptionType), "BeginCatchBlock"));

        var ret = new CatchBlock(exceptionType, forTry);

        _catchBlocks[ret] = Tuple.Create(_il.Index, -1);

        return ret;
    }

    /// <summary>
    /// Begins a catch block for the given exception type in the given exception block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public Emit<TDelegateType> BeginCatchBlock(ExceptionBlock forTry, Type exceptionType, out CatchBlock forCatch)
    {
        forCatch = BeginCatchBlock(forTry, exceptionType);

        return this;
    }

    /// <summary>
    /// Ends the given catch block.
    /// </summary>
    public Emit<TDelegateType> EndCatchBlock(CatchBlock forCatch)
    {
        if (forCatch == null)
        {
            throw new ArgumentNullException("forCatch");
        }

        if (((IOwned)forCatch).Owner != this)
        {
            FailOwnership(forCatch);
        }

        if (_mustMark)
        {
            MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
        }

        UpdateState(Wrap(new[] { new StackTransition(0) }, "EndCatchBlock"));

        var location = _catchBlocks[forCatch];

        if (location.Item2 != -1)
        {
            throw new InvalidOperationException("CatchBlock has already been ended");
        }

        _il.EndCatchBlock();

        _catchBlocks[forCatch] = Tuple.Create(location.Item1, _il.Index);

        return this;
    }

    /// <summary>
    /// Begins a finally block on the given exception block.
    ///
    /// Only one finally block can be defined per exception block, and the block cannot appear within a catch block.
    ///
    /// The given exception block must still be open.
    /// </summary>
    public Emit<TDelegateType> BeginFinallyBlock(ExceptionBlock forTry, out FinallyBlock forFinally)
    {
        forFinally = BeginFinallyBlock(forTry);

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
        if (forTry == null)
        {
            throw new ArgumentNullException("forTry");
        }

        if (((IOwned)forTry).Owner != this)
        {
            FailOwnership(forTry);
        }

        var tryBlock = _tryBlocks[forTry];

        if (tryBlock.Item2 != -1)
        {
            throw new InvalidOperationException("BeginFinallyBlock expects an unclosed exception block, but " + forTry + " is already closed");
        }

        if (_currentExceptionBlock.Count > 0 && forTry != _currentExceptionBlock.Peek())
        {
            throw new InvalidOperationException("Cannot begin FinallyBlock on " + forTry + " while inner ExceptionBlock " + _currentExceptionBlock.Peek() + " is still open");
        }

        if (_finallyBlocks.Any(kv => kv.Key.ExceptionBlock == forTry))
        {
            throw new InvalidOperationException("There can only be one finally block per ExceptionBlock, and one is already defined for " + forTry);
        }

        if (_mustMark)
        {
            MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
        }

        UpdateState(Wrap(new[] { new StackTransition(0) }, "BeginFinallyBlock"));

        var ret = new FinallyBlock(forTry);

        _il.BeginFinallyBlock();

        _finallyBlocks[ret] = Tuple.Create(_il.Index, -1);

        return ret;
    }

    /// <summary>
    /// Ends the given finally block.
    /// </summary>
    public Emit<TDelegateType> EndFinallyBlock(FinallyBlock forFinally)
    {
        if (forFinally == null)
        {
            throw new ArgumentNullException("forFinally");
        }

        if (((IOwned)forFinally).Owner != this)
        {
            FailOwnership(forFinally);
        }

        var finallyBlock = _finallyBlocks[forFinally];

        if (finallyBlock.Item2 != -1)
        {
            throw new InvalidOperationException("EndFinallyBlock expects an unclosed finally block, but " + forFinally + " is already closed");
        }

        if (_mustMark)
        {
            MarkLabel(DefineLabel(AutoNamer.Next(this, "__autolabel")));
        }

        UpdateState(Wrap(new[] { new StackTransition(0) }, "EndFinallyBlock"));

        _il.EndFinallyBlock();

        _finallyBlocks[forFinally] = Tuple.Create(finallyBlock.Item1, _il.Index);

        return this;
    }
}
