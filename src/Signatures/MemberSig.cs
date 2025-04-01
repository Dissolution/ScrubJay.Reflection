using ScrubJay.Fluent;

namespace ScrubJay.Reflection.Signatures;

public record class MemberSig
{
    [return: NotNullIfNotNull(nameof(memberInfo))]
    public static implicit operator MemberSig?(MemberInfo? memberInfo) => memberInfo is null ? null : new(memberInfo);

    public Attributes Attributes { get; init; } = [];

    public string? Name { get; set; }

    public Visibility Visibility { get; set; } = Visibility.None;
    public Access Access { get; set; } = Access.None;
    public MemberTypes MemberType { get; set; } = default!;

    public MemberSig() { }

    public MemberSig(MemberInfo memberInfo)
    {
        this.Attributes = Attribute.GetCustomAttributes(memberInfo);
        this.Name = memberInfo.Name;
        this.Visibility = memberInfo.Visibility();
        this.Access = memberInfo.Access();
        this.MemberType = memberInfo.MemberType;
    }
}

public class FluentMemberSigBuilder<TB, TM> : FluentRecordBuilder<TB, TM>
    where TB : FluentMemberSigBuilder<TB, TM>
    where TM : MemberSig, new()
{
    public FluentMemberSigBuilder(TM record) : base(record)
    {
    }
}
