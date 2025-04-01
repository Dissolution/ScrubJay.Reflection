
using ScrubJay.Sigil.Utilities;
#if !NETSTANDARD
using System.Runtime.Serialization;
#endif

namespace ScrubJay.Sigil;

/// <summary>
/// A SigilVerificationException is thrown whenever a CIL stream becomes invalid.
///
/// There are many possible causes of this including: operator type mismatches, underflowing the stack, and branching from one stack state to another.
///
/// Invalid arguments, non-sensical parameters, and other non-correctness related errors will throw more specific exceptions.
///
/// SigilVerificationException will typically include the state of the stack (or stacks) at the instruction in error.
/// </summary>
public class SigilVerificationException : Exception
{
    private readonly string[] _instructions;
    private readonly VerificationResult _verificationFailure;
    private readonly ReturnTracerResult _returnFailure;

    internal SigilVerificationException(string message, ReturnTracerResult failure, string[] instructions)
        : this(message, instructions)
    {
        _returnFailure = failure;
    }

    internal SigilVerificationException(string method, VerificationResult failure, string[] instructions)
        : this(GetMessage(method, failure), instructions)
    {
        _verificationFailure = failure;
    }

    internal SigilVerificationException(string message, string[] instructions) : base(message)
    {
        _instructions = instructions;
    }

    private static string GetMessage(string method, VerificationResult failure)
    {
        if (failure.IsStackUnderflow)
        {
            if (failure.ExpectedStackSize == 1)
            {
                return method + " expects a value on the stack, but it was empty";
            }

            return method + " expects " + failure.ExpectedStackSize + " values on the stack";
        }

        if (failure.IsTypeMismatch)
        {
            if (failure.ExpectedAtStackIndex != null)
            {
                var expected = ErrorMessageString(failure.ExpectedAtStackIndex);
                var found = ErrorMessageString(failure.Stack.ElementAt(failure.StackIndex));

                return method + " expected " + (ExtensionMethods.StartsWithVowel(expected) ? "an " : "a ") + expected + "; found " + found;
            }

            var ex = ErrorMessageString(failure.ExpectedOnStack);
            var ac = ErrorMessageString(failure.ActuallyOnStack);

            return method + " expected " + (ExtensionMethods.StartsWithVowel(ex) ? "an " : "a ") + ex + "; found " + ac;
        }

        if (failure.IsStackMismatch)
        {
            return method + " resulted in stack mismatches";
        }

        if (failure.IsStackSizeFailure)
        {
            if (failure.ExpectedStackSize == 0)
            {
                return method + " expected the stack of be empty";
            }

            if (failure.ExpectedStackSize == 1)
            {
                return method + " expected the stack to have 1 value";
            }

            return method + " expected the stack to have " + failure.ExpectedStackSize + " values";
        }

        throw new Exception("Shouldn't be possible!");
    }

    private static string ErrorMessageString(IEnumerable<TypeOnStack> types)
    {
        var names = types.Select(t => t.ToString()).OrderBy(n => n).ToArray();

        if (names.Length == 1) return names[0];

        var ret = new StringBuilder();
        ret.Append(names[0]);

        for (var i = 1; i < names.Length - 1; i++)
        {
            ret.Append(", " + names[i]);
        }

        ret.Append(", or " + names[names.Length - 1]);

        return ret.ToString();
    }

    /// <summary>
    /// Returns a string representation of any stacks attached to this exception.
    ///
    /// This is meant for debugging purposes, and should not be called during normal operation.
    /// </summary>
    public string GetDebugInfo()
    {
        var ret = new StringBuilder();

        if (_verificationFailure != null)
        {

            if (_verificationFailure.IsStackMismatch)
            {
                ret.AppendLine("Expected Stack");
                ret.AppendLine("==============");
                PrintStack(_verificationFailure.ExpectedStack, ret);

                ret.AppendLine();
                ret.AppendLine("Incoming Stack");
                ret.AppendLine("==============");
                PrintStack(_verificationFailure.IncomingStack, ret);
            }

            if (_verificationFailure.IsTypeMismatch && _verificationFailure.Stack != null)
            {
                ret.AppendLine("Stack");
                ret.AppendLine("=====");
                PrintStack(_verificationFailure.Stack, ret, "// bad value", _verificationFailure.StackIndex);
            }

            if ((_verificationFailure.IsStackUnderflow || _verificationFailure.IsStackSizeFailure) && _verificationFailure.Stack != null)
            {
                ret.AppendLine("Stack");
                ret.AppendLine("=====");
                PrintStack(_verificationFailure.Stack, ret);
            }

            ret.AppendLine();
        }

        if (_returnFailure != null)
        {
            foreach (var path in _returnFailure.FailingPaths)
            {
                ret.AppendLine("Bad Path");
                ret.AppendLine("========");
                foreach (var label in path)
                {
                    ret.AppendLine(label.Name);
                }

                ret.AppendLine();
            }
        }

        ret.AppendLine("Instructions");
        ret.AppendLine("============");

        var instrIx = _verificationFailure != null && _verificationFailure.TransitionIndex != null ? _verificationFailure.Verifier.GetInstructionIndex(_verificationFailure.TransitionIndex.Value) : -1;

        for (var i = 2; i < _instructions.Length; i++)
        {
            var line = _instructions[i];

            if (i == instrIx) line += "  // relevant instruction";

            if (!string.IsNullOrEmpty(line))
            {
                ret.AppendLine(line);
            }
        }

        return ret.ToString();
    }

    private static void PrintStack(LinqStack<List<TypeOnStack>> stack, StringBuilder sb, string mark = null, int? markAt = null)
    {
        if (stack.Count == 0)
        {
            sb.AppendLine("--empty--");
            return;
        }

        for (var i = 0; i < stack.Count; i++)
        {
            var asStr =
                string.Join(", or",
                    stack.ElementAt(i).Select(s => s.ToString()).ToArray()
                );

            if (i == markAt)
            {
                asStr += "  " + mark;
            }

            sb.AppendLine(asStr);
        }
    }



    /// <summary>
    /// Returns the message and stacks on this exception, in string form.
    /// </summary>
    public override string ToString()
    {
        return
            Message + Environment.NewLine + Environment.NewLine + GetDebugInfo();
    }
}
