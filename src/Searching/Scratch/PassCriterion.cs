namespace ScrubJay.Reflection.Searching.Scratch;

public sealed class PassCriterion<T> : ICriterion<T>
{
    public static PassCriterion<T> Any { get; } = new(true);
    public static PassCriterion<T> None { get; } = new(false);

    private readonly bool _pass;

    private PassCriterion(bool pass)
    {
        _pass = pass;
    }

    public bool Matches(T? str) => _pass;

    public override string ToString() => _pass ? nameof(Any) : nameof(None);
}