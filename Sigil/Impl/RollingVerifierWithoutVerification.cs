using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Impl;

internal class RollingVerifierWithoutVerification : RollingVerifier
{
    public RollingVerifierWithoutVerification(SigilLabel beginAt)
        : base(beginAt, strictBranchVerification: false)
    { }

    public override VerificationResult ConditionalBranch(params SigilLabel[] toLabels)
    {
        return VerificationResult.Successful();
    }

    public override LinqStack<TypeOnStack> InferStack(int ofDepth)
    {
        return null;
    }

    public override VerificationResult Mark(SigilLabel sigilLabel)
    {
        return VerificationResult.Successful();
    }

    public override VerificationResult ReThrow()
    {
        return VerificationResult.Successful();
    }

    public override VerificationResult Return()
    {
        return VerificationResult.Successful();
    }

    public override VerificationResult Throw()
    {
        return VerificationResult.Successful();
    }

    public override VerificationResult Transition(InstructionAndTransitions legalTransitions)
    {
        return VerificationResult.Successful();
    }

    public override VerificationResult UnconditionalBranch(SigilLabel to)
    {
        return VerificationResult.Successful();
    }
}
