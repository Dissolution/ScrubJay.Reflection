using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    private void ElideCasts()
    {
        foreach (var ix in _elidableCasts.OrderByDescending(_ => _))
        {
            RemoveInstruction(ix);
        }
    }

    /// <summary>
    /// Cast a reference on the stack to the given reference type.
    ///
    /// If the cast is not legal, a CastClassException will be thrown at runtime.
    /// </summary>
    public Emit<TDelegateType> CastClass<TReferenceType>()
        where TReferenceType : class
    {
        return CastClass(typeof(TReferenceType));
    }

    /// <summary>
    /// Cast a reference on the stack to the given reference type.
    ///
    /// If the cast is not legal, a CastClassException will be thrown at runtime.
    /// </summary>
    public Emit<TDelegateType> CastClass(Type referenceType)
    {
        if (referenceType == null)
        {
            throw new ArgumentNullException("referenceType");
        }

        if (referenceType.IsValueType)
        {
            throw new ArgumentException("Can only cast to ReferenceTypes, found " + referenceType);
        }

        var curIndex = _il.Index;
        bool elided = false;

        VerificationCallback before =
            (stack, baseless) =>
            {
                // Can't reason about stack unless it's completely known
                if (baseless || elided) return;

                var onStack = stack.First();

                if (onStack.All(a => ExtensionMethods.IsAssignableFrom(referenceType, a)))
                {
                    _elidableCasts.Add(curIndex);
                    elided = true;
                }
            };

        var newType = TypeOnStack.Get(referenceType);

        var transitions =
            new[]
            {
                new StackTransition(new [] { typeof(object) }, new [] { referenceType }, before: before),
            };

        UpdateState(OpCodes.Castclass, referenceType, Wrap(transitions, "CastClass"));

        return this;
    }
}
