namespace ScrubJay.Reflection.OLD.OLD.Runtime.Emission.Adaption;

public sealed record class MemberDelegate<TMember, TDelegate>
{
    public TMember Member { get; }
    
    public TDelegate Delegate { get; }

    public MemberDelegate(TMember member, TDelegate @delegate)
    {
        this.Member = member;
        this.Delegate = @delegate;
    }
}