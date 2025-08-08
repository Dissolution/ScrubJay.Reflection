namespace ScrubJay.Reflection.Exceptions;

[PublicAPI]
public class MemberException : ReflectionException
{
    public MemberInfo? Member { get; init; }

    public MemberException()
        : base()
    {
    }

    public MemberException(MemberInfo? member)
        : base()
    {
        this.Member = member;
    }

    public MemberException(MemberInfo? member, ref InterpolatedTextBuilder message)
        : base(ref message)
    {
        this.Member = member;
    }

    public MemberException(MemberInfo? member, ref InterpolatedTextBuilder message, Exception? innerException)
        : base(ref message, innerException)
    {
        this.Member = member;
    }

    public override string ToString()
    {
        return TextBuilder.Build($"{GetType():@} - {Member:@}: {Message}");
    }
}