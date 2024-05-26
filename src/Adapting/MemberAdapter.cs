using ScrubJay.Reflection.Runtime;
using static ScrubJay.GlobalHelper;
using ScrubJay.Reflection.Runtime.Emission.Adaption;
using ScrubJay.Reflection.Runtime.Naming;

namespace ScrubJay.Reflection.Adapting;

public delegate TValue GetValue<TInstance, out TValue>(ref TInstance instance);
public delegate void SetValue<TInstance, in TValue>(ref TInstance instance, TValue value);
public delegate TInstance Construct<out TInstance>(params object?[] args);



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
                return Ok();
            return Error("Static members require NoInstance for their generic instance Type");
        }
        else
        {
            // Instance has to implement member instance
            if (instanceType.Implements(member.OwnerType()))
                return Ok();
            return Error("Instance members require a generic instance Type that implements their owning Type");
        }
    }

    protected Result<Ok, Error> ValidateReturn<TReturn>(Type stackType)
    {
        if (stackType.Implements<TReturn>())
            return Ok();
        return Error("Generic return Type must be implemented by stack type");
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




public class FieldAdapter : MemberAdapter<FieldInfo>
{
    public Result<GetValue<TInstance, TValue>, Exception> TryCreateGetter<TInstance, TValue>(FieldInfo field)
    {
        if (ValidateInstance<TInstance>(field).IsError(out var instanceError))
            return GetException<GetValue<TInstance, TValue>>(field, instanceError);
        if (ValidateReturn<TValue>(field.FieldType).IsError(out var returnError))
            return GetException<GetValue<TInstance, TValue>>(field, returnError);

        var builder = RuntimeBuilder.CreateDelegateBuilder<GetValue<TInstance, TValue>>(InterpolateDeeper.Resolve($"get_{typeof(TInstance)}_{field.Name}"));
        var emitter = builder.Emitter;

        if (!field.IsStatic)
        {
            // Load instance
            
        }


        throw new NotImplementedException();
    }
}