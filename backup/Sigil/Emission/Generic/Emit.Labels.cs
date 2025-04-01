namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    // Go through and slap *_S everywhere it's needed for branches
    private void PatchBranches()
    {
        foreach (var start in _branchPatches.Keys.OrderBy(o => o))
        {
            var item = _branchPatches[start];

            var label = item.Item1;
            var patcher = item.Item2;
            var originalOp = item.Item3;

            if (!_marks.ContainsKey(label))
            {
                throw new SigilVerificationException("Usage of unmarked label " + label, _il.Instructions(_allLocals));
            }

            var stop = _marks[label];

            var distance = _il.ByteDistance(start, stop);

            if (distance >= sbyte.MinValue && distance <= sbyte.MaxValue)
            {
                if (originalOp == OpCodes.Switch)
                {
                    // No short form to be had
                    continue;
                }

                if (originalOp == OpCodes.Br)
                {
                    patcher(OpCodes.Br_S);
                    continue;
                }

                if (originalOp == OpCodes.Beq)
                {
                    patcher(OpCodes.Beq_S);
                    continue;
                }

                if (originalOp == OpCodes.Bne_Un)
                {
                    patcher(OpCodes.Bne_Un_S);
                    continue;
                }

                if (originalOp == OpCodes.Bge)
                {
                    patcher(OpCodes.Bge_S);
                    continue;
                }

                if (originalOp == OpCodes.Bge_Un)
                {
                    patcher(OpCodes.Bge_Un_S);
                    continue;
                }

                if (originalOp == OpCodes.Bgt)
                {
                    patcher(OpCodes.Bgt_S);
                    continue;
                }

                if (originalOp == OpCodes.Bgt_Un)
                {
                    patcher(OpCodes.Bgt_Un_S);
                    continue;
                }

                if (originalOp == OpCodes.Ble)
                {
                    patcher(OpCodes.Ble_S);
                    continue;
                }

                if (originalOp == OpCodes.Ble_Un)
                {
                    patcher(OpCodes.Ble_Un_S);
                    continue;
                }

                if (originalOp == OpCodes.Blt)
                {
                    patcher(OpCodes.Blt_S);
                    continue;
                }

                if (originalOp == OpCodes.Blt_Un)
                {
                    patcher(OpCodes.Blt_Un_S);
                    continue;
                }

                if (originalOp == OpCodes.Brfalse)
                {
                    patcher(OpCodes.Brfalse_S);
                    continue;
                }

                if (originalOp == OpCodes.Brtrue)
                {
                    patcher(OpCodes.Brtrue_S);
                    continue;
                }

                if (originalOp == OpCodes.Leave)
                {
                    patcher(OpCodes.Leave_S);
                    continue;
                }

                throw new Exception("Unexpected OpCode: " + originalOp);
            }
        }
    }

    /// <summary>
    /// Defines a new label.
    ///
    /// This label can be used for branching, leave, and switch instructions.
    ///
    /// A label must be marked exactly once after being defined, using the MarkLabel() method.
    /// </summary>
    public SigilLabel DefineLabel(string name = null)
    {
        name = name ?? AutoNamer.Next(this, "_label", Locals.Names, Labels.Names);

        if (_currentLabels.ContainsKey(name))
        {
            throw new InvalidOperationException("Label with name '" + name + "' already exists");
        }

        var label = _il.DefineLabel();

        var ret = new SigilLabel(this, label, name);

        _unusedLabels.Add(ret);
        _unmarkedLabels.Add(ret);

        _currentLabels[name] = ret;

        return ret;
    }

    /// <summary>
    /// Defines a new label.
    ///
    /// This label can be used for branching, leave, and switch instructions.
    ///
    /// A label must be marked exactly once after being defined, using the MarkLabel() method.
    /// </summary>
    public Emit<TDelegateType> DefineLabel(out SigilLabel sigilLabel, string name = null)
    {
        sigilLabel = DefineLabel(name);

        return this;
    }


    /// <summary>
    /// Marks a label in the instruction stream.
    ///
    /// When branching, leaving, or switching with a label control will be transfered to where it was *marked* not defined.
    ///
    /// Labels can only be marked once, and *must* be marked before creating a delegate.
    /// </summary>
    public Emit<TDelegateType> MarkLabel(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return MarkLabel(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        if (!_unmarkedLabels.Contains(sigilLabel))
        {
            throw new InvalidOperationException("label [" + sigilLabel.Name + "] has already been marked, and cannot be marked a second time");
        }

        if (_mustMark)
        {
            _mustMark = false;
        }

        var valid = _currentVerifiers.Mark(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("MarkLabel", valid, _il.Instructions(_allLocals));
        }

        _unmarkedLabels.Remove(sigilLabel);

        _il.MarkLabel(sigilLabel);

        _marks[sigilLabel] = _il.Index;

        return this;
    }

    /// <summary>
    /// Marks a label with the given name in the instruction stream.
    ///
    /// When branching, leaving, or switching with a label control will be transfered to where it was *marked* not defined.
    ///
    /// Labels can only be marked once, and *must* be marked before creating a delegate.
    /// </summary>
    public Emit<TDelegateType> MarkLabel(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return MarkLabel(Labels[name]);
    }
}
