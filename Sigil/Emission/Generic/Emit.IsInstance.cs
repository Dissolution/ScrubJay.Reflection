namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Pops a value from the stack and casts to the given type if possible pushing the result, otherwise pushes a null.
    ///
    /// This is analogous to C#'s `as` operator.
    /// </summary>
    public Emit<TDelegateType> IsInstance<T>()
    {
        return IsInstance(typeof(T));
    }

    /// <summary>
    /// Pops a value from the stack and casts to the given type if possible pushing the result, otherwise pushes a null.
    ///
    /// This is analogous to C#'s `as` operator.
    /// </summary>
    public Emit<TDelegateType> IsInstance(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        var curIndex = _il.Index;
        bool elided = false;

        VerificationCallback before =
            (stack, baseless) =>
            {
                // Can't reason about stack unless it's completely known
                if (baseless || elided) return;

                var onStack = stack.First();

                if (onStack.All(a => ExtensionMethods.IsAssignableFrom(type, a)))
                {
                    _elidableCasts.Add(curIndex);
                    elided = true;
                }
            };

        var transitions =
            new[]
            {
                new StackTransition(new[] { typeof(WildcardType) }, new [] { type }, before),
            };

        UpdateState(OpCodes.Isinst, type, Wrap(transitions, "IsInstance"));

        return this;
    }
}
