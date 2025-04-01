namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Unconditionally branches to the given label.
    /// </summary>
    public Emit<TDelegateType> Branch(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return Branch(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Br, sigilLabel, Wrap(StackTransition.None(), "Branch"), out update);

        var valid = _currentVerifiers.UnconditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("Branch", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Br, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Br);

        _mustMark = true;

        return this;
    }

    /// <summary>
    /// Unconditionally branches to the label with the given name.
    /// </summary>
    public Emit<TDelegateType> Branch(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return Branch(Labels[name]);
    }

    /// <summary>
    /// Pops two arguments from the stack, if both are equal branches to the given label.
    /// </summary>
    public Emit<TDelegateType> BranchIfEqual(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return BranchIfEqual(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        var transitions =
            new[]
            {
                new StackTransition(new [] { typeof(WildcardType), typeof(WildcardType) }, []),
            };

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Beq, sigilLabel, Wrap(transitions, "BranchIfEqual"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("BranchIfEqual", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Beq, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Beq);

        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, if both are equal branches to the label with the given name.
    /// </summary>
    public Emit<TDelegateType> BranchIfEqual(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return BranchIfEqual(Labels[name]);
    }

    /// <summary>
    /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the given label.
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfNotEqual(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return UnsignedBranchIfNotEqual(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        var transitions =
            new[]
            {
                new StackTransition(new [] { typeof(WildcardType), typeof(WildcardType) }, []),
            };

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Bne_Un, sigilLabel, Wrap(transitions, "UnsignedBranchIfNotEqual"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("UnsignedBranchIfNotEqual", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Bne_Un, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Bne_Un);

        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the label with the given name.
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfNotEqual(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return UnsignedBranchIfNotEqual(Labels[name]);
    }

    private TransitionWrapper BranchComparableTransitions(string name)
    {
        return
            Wrap(
                new[]
                {
                    new StackTransition(new [] { typeof(int), typeof(int) }, []),
                    new StackTransition(new [] { typeof(int), typeof(NativeIntType) }, []),
                    new StackTransition(new [] { typeof(NativeIntType), typeof(int) }, []),
                    new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, []),
                    new StackTransition(new [] { typeof(long), typeof(long) }, []),
                    new StackTransition(new [] { typeof(float), typeof(float) }, []),
                    new StackTransition(new [] { typeof(double), typeof(double) }, []),
                },
                name
            );
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value.
    /// </summary>
    public Emit<TDelegateType> BranchIfGreaterOrEqual(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return BranchIfGreaterOrEqual(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Bge, sigilLabel, BranchComparableTransitions("BranchIfGreaterOrEqual"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("BranchIfGreaterOrEqual", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Bge, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Bge);


        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than or equal to the first value.
    /// </summary>
    public Emit<TDelegateType> BranchIfGreaterOrEqual(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return BranchIfGreaterOrEqual(Labels[name]);
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value (when treated as unsigned values).
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfGreaterOrEqual(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return UnsignedBranchIfGreaterOrEqual(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Bge_Un, sigilLabel, BranchComparableTransitions("UnsignedBranchIfGreaterOrEqual"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("UnsignedBranchIfGreaterOrEqual", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Bge_Un, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Bge_Un);

        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than or equal to the first value (when treated as unsigned values).
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfGreaterOrEqual(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return UnsignedBranchIfGreaterOrEqual(Labels[name]);
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value.
    /// </summary>
    public Emit<TDelegateType> BranchIfGreater(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return BranchIfGreater(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Bgt, sigilLabel, BranchComparableTransitions("BranchIfGreater"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("BranchIfGreater", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Bgt, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Bgt);

        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than the first value.
    /// </summary>
    public Emit<TDelegateType> BranchIfGreater(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return BranchIfGreater(Labels[name]);
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value (when treated as unsigned values).
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfGreater(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return UnsignedBranchIfGreater(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Bgt_Un, sigilLabel, BranchComparableTransitions("UnsignedBranchIfGreater"), out update);

        var valid = _currentVerifiers.UnconditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("UnsignedBranchIfGreater", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Bgt_Un, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Bgt_Un);

        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than the first value (when treated as unsigned values).
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfGreater(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return UnsignedBranchIfGreater(Labels[name]);
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value.
    /// </summary>
    public Emit<TDelegateType> BranchIfLessOrEqual(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return BranchIfLessOrEqual(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Ble, sigilLabel, BranchComparableTransitions("BranchIfLessOrEqual"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("BranchIfLessOrEqual", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Ble, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Ble);

        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than or equal to the first value.
    /// </summary>
    public Emit<TDelegateType> BranchIfLessOrEqual(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return BranchIfLessOrEqual(Labels[name]);
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value (when treated as unsigned values).
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfLessOrEqual(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return UnsignedBranchIfLessOrEqual(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Ble_Un, sigilLabel, BranchComparableTransitions("UnsignedBranchIfLessOrEqual"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("UnsignedBranchIfLessOrEqual", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Ble_Un, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Ble_Un);

        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than or equal to the first value (when treated as unsigned values).
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfLessOrEqual(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return UnsignedBranchIfLessOrEqual(Labels[name]);
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value.
    /// </summary>
    public Emit<TDelegateType> BranchIfLess(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return BranchIfLess(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Blt, sigilLabel, BranchComparableTransitions("BranchIfLess"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("BranchIfLess", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Blt, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Blt);

        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than the first value.
    /// </summary>
    public Emit<TDelegateType> BranchIfLess(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return BranchIfLess(Labels[name]);
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value (when treated as unsigned values).
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfLess(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return UnsignedBranchIfLess(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Blt_Un, sigilLabel, BranchComparableTransitions("UnsignedBranchIfLess"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("UnsignedBranchIfLess", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Blt_Un, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Blt_Un);

        return this;
    }

    /// <summary>
    /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than the first value (when treated as unsigned values).
    /// </summary>
    public Emit<TDelegateType> UnsignedBranchIfLess(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return UnsignedBranchIfLess(Labels[name]);
    }

    /// <summary>
    /// Pops one argument from the stack, branches to the given label if the value is false.
    ///
    /// A value is false if it is zero or null.
    /// </summary>
    public Emit<TDelegateType> BranchIfFalse(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return BranchIfFalse(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        var transitions =
            new []
            {
                new StackTransition(new [] { typeof(WildcardType) }, []),
            };

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Brfalse, sigilLabel, Wrap(transitions, "BranchIfFalse"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("BranchIfFalse", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Brfalse, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Brfalse);

        return this;
    }

    /// <summary>
    /// Pops one argument from the stack, branches to the label with the given name if the value is false.
    ///
    /// A value is false if it is zero or null.
    /// </summary>
    public Emit<TDelegateType> BranchIfFalse(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return BranchIfFalse(Labels[name]);
    }

    /// <summary>
    /// Pops one argument from the stack, branches to the given label if the value is true.
    ///
    /// A value is true if it is non-zero or non-null.
    /// </summary>
    public Emit<TDelegateType> BranchIfTrue(SigilLabel sigilLabel)
    {
        if (sigilLabel == null)
        {
            throw new ArgumentNullException("sigilLabel");
        }

        if (((IOwned)sigilLabel).Owner != this)
        {
            if (((IOwned)sigilLabel).Owner is DisassembledOperations<TDelegateType>)
            {
                return BranchIfTrue(sigilLabel.Name);
            }

            FailOwnership(sigilLabel);
        }

        _unusedLabels.Remove(sigilLabel);

        var transitions =
            new[]
            {
                new StackTransition(new [] { typeof(WildcardType) }, []),
            };

        UpdateOpCodeDelegate update;
        UpdateState(OpCodes.Brtrue, sigilLabel, Wrap(transitions, "BranchIfTrue"), out update);

        var valid = _currentVerifiers.ConditionalBranch(sigilLabel);
        if (!valid.Success)
        {
            throw new SigilVerificationException("BranchIfTrue", valid, _il.Instructions(_allLocals));
        }

        _branches.Add(Tuple.Create(OpCodes.Brtrue, sigilLabel, _il.Index));

        _branchPatches[_il.Index] = Tuple.Create(sigilLabel, update, OpCodes.Brtrue);

        return this;
    }

    /// <summary>
    /// Pops one argument from the stack, branches to the label with the given name if the value is true.
    ///
    /// A value is true if it is non-zero or non-null.
    /// </summary>
    public Emit<TDelegateType> BranchIfTrue(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return BranchIfTrue(Labels[name]);
    }
}
