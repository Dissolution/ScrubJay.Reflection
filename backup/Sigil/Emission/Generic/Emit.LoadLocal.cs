namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Loads the value in the given local onto the stack.
    ///
    /// To create a local, use DeclareLocal().
    /// </summary>
    public Emit<TDelegateType> LoadLocal(SigilLocal sigilLocal)
    {
        if (sigilLocal == null)
        {
            throw new ArgumentNullException("sigilLocal");
        }

        if (((IOwned)sigilLocal).Owner != this)
        {
            if (((IOwned)sigilLocal).Owner is DisassembledOperations<TDelegateType>)
            {
                return LoadLocal(sigilLocal.Name);
            }

            FailOwnership(sigilLocal);
        }

        _unusedLocals.Remove(sigilLocal);

        switch (sigilLocal.Index)
        {
            case 0: UpdateState(OpCodes.Ldloc_0, Wrap(StackTransition.Push(sigilLocal.StackType), "LoadLocal")); return this;
            case 1: UpdateState(OpCodes.Ldloc_1, Wrap(StackTransition.Push(sigilLocal.StackType), "LoadLocal")); return this;
            case 2: UpdateState(OpCodes.Ldloc_2, Wrap(StackTransition.Push(sigilLocal.StackType), "LoadLocal")); return this;
            case 3: UpdateState(OpCodes.Ldloc_3, Wrap(StackTransition.Push(sigilLocal.StackType), "LoadLocal")); return this;
        }

        if (sigilLocal.Index >= byte.MinValue && sigilLocal.Index <= byte.MaxValue)
        {
            UpdateState(OpCodes.Ldloc_S, (byte)sigilLocal.Index, Wrap(StackTransition.Push(sigilLocal.StackType), "LoadLocal"));
            return this;
        }

        UpdateState(OpCodes.Ldloc, sigilLocal, Wrap(StackTransition.Push(sigilLocal.StackType), "LoadLocal"));

        return this;
    }

    /// <summary>
    /// Loads the value in the local with the given name onto the stack.
    /// </summary>
    public Emit<TDelegateType> LoadLocal(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return LoadLocal(Locals[name]);
    }
}
