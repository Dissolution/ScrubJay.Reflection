using ScrubJay.Reflection.Runtime.Emission.Adaption;
using ScrubJay.Reflection.Runtime.Naming;

namespace ScrubJay.Reflection.Adapting;

public class MemberAdapter
{
    
}

public class MemberAdapter<TMember> : MemberAdapter
    where TMember : MemberInfo
{
    protected Result<Ok, Error> ValidateInstance<TInstance>(TMember member)
    {
        Type instanceType = typeof(TInstance);
        
        if (member.IsStatic())
        {
            if (instanceType == typeof(NoInstance) || instanceType == typeof(NoInstance).MakeByRefType())
                return GlobalHelper.Ok();
            return GlobalHelper.Error("Static members require NoInstance for their generic instance Type");
        }
        else
        {
            // Instance has to implement member instance
            if (instanceType.Implements(member.OwnerType()))
                return GlobalHelper.Ok();
            return GlobalHelper.Error("Instance members require a generic instance Type that implements their owning Type");
        }
    }

    protected Result<Ok, Error> ValidateReturn<TReturn>(Type stackType)
    {
        if (stackType.Implements<TReturn>())
            return GlobalHelper.Ok();
        return GlobalHelper.Error("Generic return Type must be implemented by stack type");
    }

    protected Reflexception GetException<TDelegate>(TMember member, Error error)
    {
        return new Reflexception(error.Message)
        {
            Data =
            {
                ["Member"] = member,
                ["DelegateType"] = typeof(TDelegate).Dump(),
            },
        };
    }
    
    protected Reflexception GetException<TInstance, TValue>(TMember member, Error error)
    {
        return new Reflexception(error.Message)
        {
            Data =
            {
                ["Member"] = member,
                ["InstanceType"] = typeof(TInstance).Dump(),
                ["ValueType"] = typeof(TValue).Dump(),
            },
        };
    }
    
    
}