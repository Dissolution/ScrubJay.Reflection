namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a value off the stack and branches to the label at the index of that value in the given labels.
    ///
    /// If the value is out of range, execution falls through to the next instruction.
    /// </summary>
    public Emit<TDelegateType> Switch(params SigilLabel[] labels)
    {
        if (labels == null)
        {
            throw new ArgumentNullException("labels");
        }

        if (labels.Length == 0)
        {
            throw new ArgumentException("labels must have at least one element");
        }

        if (labels.Any(l => ((IOwned)l).Owner is DisassembledOperations<TDelegateType>))
        {
            return
                Switch(labels.Select(l => l.Name).ToArray());
        }

        foreach (var label in labels)
        {
            if (((IOwned)label).Owner != this)
            {
                FailOwnership(label);
            }
        }

        foreach (var label in labels)
        {
            _unusedLabels.Remove(label);
        }

        var transitions =
            new[]
            {
                new StackTransition(new [] { typeof(int) }, []),
                new StackTransition(new [] { typeof(NativeIntType) }, []),
            };

        var labelsCopy = (labels).Select(l => l).ToArray();

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Switch, labelsCopy, Wrap(transitions, "Switch"), out update);

        var valid = _currentVerifiers.ConditionalBranch(labels);
        if (!valid.Success)
        {
            throw new SigilVerificationException("Switch", valid, _il.Instructions(_allLocals));
        }

        foreach (var label in labels)
        {
            _branches.Add(Tuple.Create(OpCodes.Switch, label, _il.Index));
            _branchPatches[_il.Index] = Tuple.Create(label, update, OpCodes.Switch);
        }

        return this;
    }

    /// <summary>
    /// Pops a value off the stack and branches to the label at the index of that value in the given label names.
    ///
    /// If the value is out of range, execution falls through to the next instruction.
    /// </summary>
    public Emit<TDelegateType> Switch(params string[] names)
    {
        if (names == null) throw new ArgumentNullException("names");

        var lNames = names;

        if (lNames.Any(n => n == null)) throw new ArgumentException("no label can be null");

        return Switch(lNames.Select(n => Labels[n]).ToArray());
    }
}
