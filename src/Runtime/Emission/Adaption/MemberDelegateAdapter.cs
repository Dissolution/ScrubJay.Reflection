namespace ScrubJay.Reflection.Runtime.Emission.Adaption;

public abstract class MemberDelegateAdapter<TMember, TDelegate>
    where TMember : MemberInfo
    where TDelegate : Delegate
{
    public abstract Result<TDelegate, Exception> TryAdapt(TMember member);

    protected bool IsStaticInstanceType<TInstance>() => IsStaticInstanceType(typeof(TInstance));
    protected bool IsStaticInstanceType(Type? instanceType)
    {
        return instanceType is null || instanceType.IsStatic() || instanceType == typeof(NoInstance) || instanceType == typeof(object);
    }

    protected virtual ArgumentException GetArgAdaptException(TMember member, 
        string? message = null,
        [CallerArgumentExpression(nameof(member))] string? memberName = null)
    {
        
        return new ArgumentException($"Cannot adapt {typeof(TDelegate)} to call {typeof(TMember)} '{member}'", memberName)
        {
            Data =
            {
                { "MemberType", typeof(TMember) },
                { "Member", member },
                { "DelegateType", typeof(TDelegate) },
            },
        };
    }


    protected bool CanConvert(Variable input, Variable output)
    {
        if (input.CanLoad && input.Type == output.Type)
        {
            return true;
        }





        return false;
    }
}