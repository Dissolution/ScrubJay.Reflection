using ScrubJay.Reflection.Adapting.Arguments;
using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.MosDef;

using Emit = System.Action<ScrubJay.Reflection.IL.Emission.Emitter>;

namespace ScrubJay.Reflection.Adapting;

public abstract class MemberDelegateAdapter
{
    protected readonly DynamicILMethod _dynamicILMethod; 
    
    public DynamicILMethod Builder => _dynamicILMethod;
    public DelegateInfo DelegateInfo { get; }

    protected MemberDelegateAdapter(DelegateInfo delegateInfo)
    {
        DelegateInfo = delegateInfo;
        _dynamicILMethod = RuntimeBuilder.BuildDynamicMethod(delegateInfo);
    }

    protected MemberDelegateAdapter(DelegateInfo delegateInfo,DynamicILMethod dynamicILMethod)
    {
        _dynamicILMethod = dynamicILMethod;
        DelegateInfo = delegateInfo;
    }

    protected abstract Result<Emit> CanAdapt();
    
    public Result<Delegate> TryAdapt()
    {
        if (!CanAdapt().IsOkWithError(out var emit, out var error))
            return error;

        try
        {
            emit.Invoke(Builder.Emitter);
        }
        catch (Exception ex)
        {
            return ex;
        }

        return Builder.TryCreateDelegate();
    }
}

public abstract class MemberDelegateAdapter<M> : MemberDelegateAdapter
    where M : MemberInfo
{
    public M Member { get; }

    protected MemberDelegateAdapter(M member, DelegateInfo delegateInfo) 
        : base(delegateInfo)
    {
        this.Member = member;
    }

    protected MemberDelegateAdapter(M member, DelegateInfo delegateInfo, DynamicILMethod dynamicILMethod) 
        : base(delegateInfo, dynamicILMethod)
    {
        this.Member = member;
    }
    
    protected Exception GetAdaptError(M? member,
        string? message = null,
        [CallerArgumentExpression(nameof(member))]
        string? memberName = null)
    {
        string exMessage = TextBuilder.New
            .Append("Cannot adapt ")
            .Render(DelegateInfo)
            .Append(" to interface with ")
            .IfNotNull(member,
                static (tb,mem) => tb
                    .Append(mem.MemberType)
                    .Append(' ')
                    .AppendMember(mem),
                static tb => tb
                    .Append("null ")
                    .AppendType(typeof(M)))
            .IfNotNull(message,
                static (tb, msg) => tb.Append(": ").Append(msg))
            .ToStringAndDispose();

        return new ArgumentException(exMessage, memberName)
        {
            Data =
            {
                {nameof(DelegateInfo), DelegateInfo},
                {nameof(Member), Member},
            },
        };
    }
    
    protected Result<(bool Skip, Emit Emit)> TryLoadInstance()
    {
        // static?
        if (Member.IsStatic())
        {
            if (Builder.ParameterCount == 0)
            {
                // do nothing
                return Ok(false);
            }
            
            // check if the first param is something we can skip
            var firstParam = Builder.Parameters[0];

            // anything marked with [Instance] we can skip
            if (firstParam.HasAttribute<InstanceAttribute>())
                return Ok(true);
            
            // ref Unit and ref None are also markers
            var (k, j) = firstParam;
            if (k.HasFlags(TRK.Ref) && (j == typeof(Unit) || j == typeof(None)))
                return Ok(true);
            
            // skip nothing
            return Ok(false);
        }
        else
        {
            if (Builder.ParameterCount == 0)
            {
                return GetAdaptError(Member, "Delegate does not have required instance parameter");
            }

            Argument instance;
            var owner = Member.DeclaringType!;
            if (owner.IsValueType)
            {
                instance = owner.MakeByRefType();
            }
            else
            {
                instance = owner;
            }
            
            var firstParam = Builder.Parameters[0];

            if (!firstParam.HasAttribute<InstanceAttribute>())
                throw new InvalidOperationException();
            
            // special handling
            if (firstParam.IsParams())
                throw new NotImplementedException();
            
            var canConvert = ArgumentConverter.CanConvert(firstParam, instance);
            if (canConvert.IsOk(out var emit))
            {
                return Ok(true, emit);
            }

            return GetAdaptError(Member, "Delegate Instance parameter cannot be converted to member instance type");
        }

        
        static Result<(bool,Emit)> Ok(bool skip, Emit? emit = null)
        {
            return Ok<(bool, Emit)>((skip, emit ?? (_ => { })));
        }
    }
}

public abstract class MemberDelegateAdapter<M, D> : MemberDelegateAdapter<M>
    where M : MemberInfo
    where D : Delegate
{
    public new DynamicILMethod<D> Builder => (DynamicILMethod<D>)_dynamicILMethod;
    
    protected MemberDelegateAdapter(M member, string? name = null) 
        : base(member, DelegateInfo.Create<D>(name), RuntimeBuilder.BuildDynamicMethod<D>())
    {
        
    }

    public new Result<D> TryAdapt()
    {
        if (!CanAdapt().IsOkWithError(out var emit, out var error))
            return error;

        try
        {
            emit.Invoke(Builder.Emitter);
        }
        catch (Exception ex)
        {
            return ex;
        }

        return Builder.TryCreateDelegate();
    }
}



public abstract class MemberDelegateAdapter<S, M, D>
    where S : MemberDelegateAdapter<S, M, D>
    where M : MemberInfo
    where D : Delegate
{
    public static S Instance { get; } = Activator.CreateInstance<S>();
    
    public abstract Result<D> TryAdapt([NotNullWhen(true)] M? member);

    protected static Exception GetEx(M? member,
        string? message = null,
        [CallerArgumentExpression(nameof(member))]
        string? memberName = null)
    {
        string exMessage = TextBuilder.New
            .Append("Cannot adapt ")
            .AppendType(typeof(D))
            .Append(" to interact with ")
            .IfNotNull(member,
                static (tb,mem) => tb
                    .Append(mem.MemberType)
                    .Append(' ')
                    .AppendMember(mem),
                static tb => tb
                    .Append("null ")
                    .AppendType(typeof(M)))
            .IfNotNull(message,
                static (tb, msg) => tb.Append(": ").Append(msg))
            .ToStringAndDispose();

        return new ArgumentException(exMessage, memberName)
        {
            Data =
            {
                { "MemberType", typeof(M) },
                { "Member", member },
                { "DelegateType", typeof(D) },
            },
        };
    }


    protected static bool IsAssertStatic(MemberInfo member, Type instanceType)
    {
        if (member.IsStatic())
        {
            if (instanceType == typeof(Unit) || instanceType == typeof(None))
            {
                return true;
            }
            throw new ArgumentException();
        }
        return false;
    }
   
}
