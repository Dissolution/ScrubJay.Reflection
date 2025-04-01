namespace ScrubJay.Sigil.Impl;

internal class TransitionWrapper
{
    public List<StackTransition> Transitions { get; private set; }
    public string MethodName { get; private set; }

    private TransitionWrapper() { }

    public static TransitionWrapper Get(string name, IEnumerable<StackTransition> transitions)
    {
        return new TransitionWrapper { MethodName = name, Transitions = new List<StackTransition>(transitions) };
    }
}
