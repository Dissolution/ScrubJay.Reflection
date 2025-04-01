namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a value off the stack and stores it into the argument to the current method identified by index.
    /// </summary>
    public Emit<TDelegateType> StoreArgument(ushort index)
    {
        if (_parameterTypes.Length == 0)
        {
            throw new InvalidOperationException("Delegate of type " + typeof(TDelegateType) + " takes no parameters");
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

            UpdateState(OpCodes.Starg_S, asByte, Wrap(StackTransition.Pop(_parameterTypes[index]), "StoreArgument"));
            return this;
        }

        short asShort;
        unchecked
        {
            asShort = (short)index;
        }

        UpdateState(OpCodes.Starg, asShort, Wrap(StackTransition.Pop(_parameterTypes[index]), "StoreArgument"));

        return this;
    }
}