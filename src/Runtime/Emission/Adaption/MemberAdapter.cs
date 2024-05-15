namespace ScrubJay.Reflection.Runtime.Emission.Adaption;

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



public delegate TValue Getter<TInstance, out TValue>(ref TInstance? instance);

public class FieldGetterAdapter<TInstance, TValue> : MemberDelegateAdapter<FieldInfo, Getter<TInstance, TValue>>
{
    public override Result<Getter<TInstance, TValue>, Exception> TryAdapt(FieldInfo field)
    {
        if (field is null)
            return new ArgumentNullException(nameof(field));
        
        // static fields
        if (field.IsStatic)
        {
            // has to have a 'static' instance type
            if (!IsStaticInstanceType<TInstance>())
                return GetArgAdaptException(field);
            
            // return type has to be compat
            //if (CanConvert(new Variable.Field(field), new Variable.Stack(typeof(TValue))))
                
        }

        throw new NotImplementedException();
    }
}