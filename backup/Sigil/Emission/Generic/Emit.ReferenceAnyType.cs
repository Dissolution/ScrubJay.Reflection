// TODO: see https://github.com/dotnet/corefx/issues/4543 item 4
#if !NETSTANDARD1_5
namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Converts a TypedReference on the stack into a RuntimeTypeHandle for the type contained with it.
    ///
    /// __makeref(int) on the stack would become the RuntimeTypeHandle for typeof(int), for example.
    /// </summary>
    public Emit<TDelegateType> ReferenceAnyType()
    {
        var transitions =
            new[] {
                new StackTransition(new[] { typeof(TypedReference) }, new [] { typeof(RuntimeTypeHandle) }),
            };

        UpdateState(OpCodes.Refanytype, Wrap(transitions, "ReferenceAnyType"));

        return this;
    }
}
#endif
