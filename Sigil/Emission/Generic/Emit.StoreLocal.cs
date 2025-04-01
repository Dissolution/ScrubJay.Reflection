namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a value off the stack and stores it into the given local.
    ///
    /// To create a local, use DeclareLocal().
    /// </summary>
    public Emit<TDelegateType> StoreLocal(SigilLocal sigilLocal)
    {
        if (sigilLocal == null)
        {
            throw new ArgumentNullException("sigilLocal");
        }

        if (((IOwned)sigilLocal).Owner != this)
        {
            if (((IOwned)sigilLocal).Owner is DisassembledOperations<TDelegateType>)
            {
                return StoreLocal(sigilLocal.Name);
            }

            FailOwnership(sigilLocal);
        }

        _unusedLocals.Remove(sigilLocal);

        switch (sigilLocal.Index)
        {
            case 0: UpdateState(OpCodes.Stloc_0, Wrap(StackTransition.Pop(sigilLocal.StackType), "StoreLocal")); return this;
            case 1: UpdateState(OpCodes.Stloc_1, Wrap(StackTransition.Pop(sigilLocal.StackType), "StoreLocal")); return this;
            case 2: UpdateState(OpCodes.Stloc_2, Wrap(StackTransition.Pop(sigilLocal.StackType), "StoreLocal")); return this;
            case 3: UpdateState(OpCodes.Stloc_3, Wrap(StackTransition.Pop(sigilLocal.StackType), "StoreLocal")); return this;
        }

        if (sigilLocal.Index >= byte.MinValue && sigilLocal.Index <= byte.MaxValue)
        {
            byte asByte;
            unchecked
            {
                asByte = (byte)sigilLocal.Index;
            }

            UpdateState(OpCodes.Stloc_S, asByte, Wrap(StackTransition.Pop(sigilLocal.StackType), "StoreLocal"));
            return this;
        }

        UpdateState(OpCodes.Stloc, sigilLocal, Wrap(StackTransition.Pop(sigilLocal.StackType), "StoreLocal"));

        return this;
    }

    /// <summary>
    /// Pops a value off the stack and stores it in the local with the given name.
    /// </summary>
    public Emit<TDelegateType> StoreLocal(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return StoreLocal(Locals[name]);
    }
}
