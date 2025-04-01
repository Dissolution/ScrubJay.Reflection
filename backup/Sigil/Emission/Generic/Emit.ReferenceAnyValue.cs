// TODO: see https://github.com/dotnet/corefx/issues/4543 item 4
#if !NETSTANDARD1_5
namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.
    ///
    /// __makeref(int) on the stack would become an int&amp;, for example.
    /// </summary>
    public Emit<TDelegateType> ReferenceAnyValue<T>()
    {
        return ReferenceAnyValue(typeof(T));
    }

    /// <summary>
    /// Converts a TypedReference on the stack into a reference to the contained object, given the type contained in the TypedReference.
    ///
    /// __makeref(int) on the stack would become an int&amp;, for example.
    /// </summary>
    public Emit<TDelegateType> ReferenceAnyValue(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        var transitions =
            new[]
            {
                new StackTransition(new[] { typeof(TypedReference) }, new[] { type.MakeByRefType() }),
            };

        UpdateState(OpCodes.Refanyval, type, Wrap(transitions, "ReferenceAnyValue"));

        return this;
    }
}
#endif
