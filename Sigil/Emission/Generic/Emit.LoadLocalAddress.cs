namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pushes a pointer to the given local onto the stack.
    ///
    /// To create a local, use DeclareLocal.
    /// </summary>
    public Emit<TDelegateType> LoadLocalAddress(SigilLocal sigilLocal)
    {
        if (sigilLocal == null)
        {
            throw new ArgumentNullException("sigilLocal");
        }

        if (((IOwned)sigilLocal).Owner != this)
        {
            if (((IOwned)sigilLocal).Owner is DisassembledOperations<TDelegateType>)
            {
                return LoadLocalAddress(sigilLocal.Name);
            }

            FailOwnership(sigilLocal);
        }

        _unusedLocals.Remove(sigilLocal);

        var type = sigilLocal.StackType.Type;

        var ptrType = type.MakePointerType();

        if (sigilLocal.Index >= byte.MinValue && sigilLocal.Index <= byte.MaxValue)
        {
            byte asByte;
            unchecked
            {
                asByte = (byte)sigilLocal.Index;
            }

            UpdateState(OpCodes.Ldloca_S, asByte, Wrap(StackTransition.Push(ptrType), "LoadLocalAddress"));
            return this;
        }

        short asShort;
        unchecked
        {
            asShort = (short)sigilLocal.Index;
        }

        UpdateState(OpCodes.Ldloca, asShort, Wrap(StackTransition.Push(ptrType), "LoadLocalAddress"));

        return this;
    }

    /// <summary>
    /// Pushes a pointer to the local with the given name onto the stack.
    /// </summary>
    public Emit<TDelegateType> LoadLocalAddress(string name)
    {
        if (name == null) throw new ArgumentNullException("name");

        return LoadLocalAddress(Locals[name]);
    }
}
