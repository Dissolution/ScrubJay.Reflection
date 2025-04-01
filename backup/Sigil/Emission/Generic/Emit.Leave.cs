namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Leave an exception or catch block, branching to the given label.
    ///
    /// This instruction empties the stack.
    /// </summary>
    public Emit<TDelegateType> Leave(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return Leave(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        if (!_tryBlocks.Any(t => t.Value.Item2 == -1) && !_catchBlocks.Any(c => c.Value.Item2 == -1))
        {
            throw new InvalidOperationException("Leave can only be used within an exception or catch block");
        }

        // Note that Leave *always* nuked the stack; nothing survies exiting an exception block
        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Leave, sigilLabel, Wrap(new[] { new StackTransition(new [] { typeof(PopAllType) }, []) }, "Leave"), out update);

        _branches.Add(Tuple.Create(OpCodes.Leave, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Leave);
        _mustMark = true;

        var valid = _currentVerifiers.UnconditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("Leave", valid, _il.Instructions(_allLocals));
        }

        return this;
    }

    /// <summary>
    /// Leave an exception or catch block, branching to the label with the given name.
    ///
    /// This instruction empties the stack.
    /// </summary>
    public Emit<TDelegateType> Leave(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return Leave(Labels[name]);
    }
}
