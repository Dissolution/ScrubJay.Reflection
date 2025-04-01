namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Loads a pointer to the argument at index (starting at zero) onto the stack.
    /// </summary>
    public Emit<TDelegateType> LoadArgumentAddress(ushort index)
    {
        if (_parameterTypes.Length == 0)
        {
            throw new ArgumentException("Delegate of type " + typeof(TDelegateType) + " takes no parameters");
        }

        if (index >= _parameterTypes.Length)
        {
            throw new ArgumentException("index must be between 0 and " + (_parameterTypes.Length - 1) + ", inclusive");
        }

        if (index >= byte.MinValue && index <= byte.MaxValue)
        {
            byte asByte;
            unchecked
            {
                asByte = (byte)index;
            }

            UpdateState(OpCodes.Ldarga_S, asByte, Wrap(StackTransition.Push(_parameterTypes[index].MakePointerType()), "LoadArgumentAddress"));

            return this;
        }

        short asShort;
        unchecked
        {
            asShort = (short)index;
        }

        UpdateState(OpCodes.Ldarga, asShort, Wrap(StackTransition.Push(_parameterTypes[index].MakePointerType()), "LoadArgumentAddress"));

        return this;
    }
}