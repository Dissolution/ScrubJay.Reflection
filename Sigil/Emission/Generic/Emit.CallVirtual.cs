using ScrubJay.Sigil.Extensions;
using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Calls the given method virtually.  Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.
    ///
    /// The `this` reference should appear before any arguments (deepest in the stack).
    ///
    /// The method invoked at runtime is determined by the type of the `this` reference.
    ///
    /// If the method invoked shouldn't vary (or if the method is static), use Call instead.
    /// </summary>
    public Emit<TDelegateType> CallVirtual(MethodInfo method, Type constrained = null, Type[] arglist = null)
    {
        if (method == null)
        {
            throw new ArgumentNullException("method");
        }

        if (method.IsStatic)
        {
            throw new ArgumentException("Only non-static methods can be called using CallVirtual, found " + method);
        }

        if (HasFlag(method.CallingConvention, CallingConventions.VarArgs) && !HasFlag(method.CallingConvention, CallingConventions.Standard))
        {
            if (arglist == null)
            {
                throw new InvalidOperationException("When calling a VarArgs method, arglist must be set");
            }
        }

        var expectedParams = (method.GetParameters()).Select(s => TypeOnStack.Get(s.ParameterType)).ToList();

        var declaring = method.DeclaringType;

        if (declaring.IsValueType)
        {
            declaring = declaring.MakePointerType();
        }

        // "this" parameter
        expectedParams.Insert(0, TypeOnStack.Get(declaring));

        if (arglist != null)
        {
            expectedParams.AddRange(arglist.Select(t => TypeOnStack.Get(t)));
        }

        var resultType = method.ReturnType == typeof(void) ? null : TypeOnStack.Get(method.ReturnType);

        // Shove the constrained prefix in if it's supplied
        if (constrained != null)
        {
            UpdateState(OpCodes.Constrained, constrained, Wrap(StackTransition.None(), "CallVirtual"));
        }

        IEnumerable<StackTransition> transitions;

        if (resultType != null)
        {
            transitions =
                new[]
                {
                    new StackTransition(expectedParams.Reversed(), new [] { resultType }),
                };
        }
        else
        {
            transitions =
                new[]
                {
                    new StackTransition(expectedParams.Reversed(), new TypeOnStack[0]),
                };
        }

        UpdateState(OpCodes.Callvirt, method, (method.GetParameters()).Select(s => s.ParameterType), Wrap(transitions, "CallVirtual"), arglist: arglist);

        return this;
    }
}
