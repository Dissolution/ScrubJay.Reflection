using ScrubJay.Reflection.Adapting.Arguments;
using ScrubJay.Reflection.IL.Emission;
using Emit = System.Action<ScrubJay.Reflection.IL.Emission.Emitter>;

namespace ScrubJay.Reflection.Adapting;

[PublicAPI]
public abstract class DelegateToMethodAdapter : DelegateToMemberAdapter,
    IDelegateToMemberAdapter<MethodBase>
{
    private static Result<(bool Skip, Emit Emit)> TryLoadInstance(MethodBase method, DelegateInfo delInfo)
    {
        // static?
        if (method.IsStatic)
        {
            if (delInfo.ParameterCount == 0)
            {
                // do nothing
                return Ok(false);
            }
            
            // check if the first param is something we can skip
            var firstParam = delInfo.Parameters[0];

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
            if (delInfo.ParameterCount == 0)
            {
                return GetError(method, delInfo, "Delegate does not have required instance parameter");
            }

            Argument instance;
            var owner = method.DeclaringType!;
            if (owner.IsValueType)
            {
                instance = owner.MakeByRefType();
            }
            else
            {
                instance = owner;
            }
            
            var firstParam = delInfo.Parameters[0];

            if (!firstParam.HasAttribute<InstanceAttribute>())
                throw new InvalidOperationException();
            
            // special handling
            if (firstParam.IsParams())
                throw new NotImplementedException();
            
            var canConvert = ArgumentCaster.LoadCastStore(firstParam, instance);
            if (canConvert.IsOk(out var emit))
            {
                return Ok(true, emit);
            }

            return GetError(method, delInfo, "Delegate Instance parameter cannot be adapter to method instance");
        }

        
        static Result<(bool,Emit)> Ok(bool skip, Emit? emit = null)
        {
            return Ok<(bool, Emit)>((skip, emit ?? (_ => { })));
        }
    }

    private static Result<Emit> TryLoadArgs(MethodBase method, DelegateInfo delInfo, bool skip)
    {
        ReadOnlySpan<ParameterInfo> builderParameters = delInfo.Parameters;
        if (skip)
        {
            builderParameters = builderParameters[1..];
        }

        ReadOnlySpan<ParameterInfo> methodParameters = method.GetParameters();

        Emissions emissions = [];
        
        int i = 0;
        while (true)
        {
            if (i >= builderParameters.Length)
            {
                if (i >= methodParameters.Length)
                {
                    // we're done
                    return Ok<Emit>(emissions.Combine());
                }
                else
                {
                    // there are method arguments left to fulfill
                    goto methodExcess;
                }
            }
            
            if (i >= methodParameters.Length)
            {
                // there are builder parameters left!?
                goto delegateExcess;
            }
            
            var builderParam = builderParameters[i];
            var methodParam = methodParameters[i];
            
            var result = ArgumentCaster.LoadCastStore(builderParam, new StackArgument(methodParam.ParameterType));
            if (!result.IsOkWithError(out var paramEmit, out var error))
                return error;

            emissions.Add(paramEmit);

            i++;
        }
        
        
        methodExcess:
        delegateExcess:
        throw new NotImplementedException();
    }

    private static Result<Emit> TryCall(MethodBase method)
    {
        if (method is ConstructorInfo ctor)
            return Ok<Emit>(emitter => emitter.Newobj(ctor));
        if (method is MethodInfo meth)
            return Ok<Emit>(emitter => emitter.Call(meth));
        return new ArgumentException(null, nameof(method));
    }
    
    private static Result<Emit> TryStoreReturn(MethodBase method, DelegateInfo delInfo)
    {
        return ArgumentCaster.LoadCastStore(method.ReturnType(), delInfo.ReturnType);
    }
    
    
    public static Result<D> TryAdapt<D>(MethodBase method)
        where D : Delegate
    {
        if (method is null)
            return new ArgumentNullException(nameof(method));

        DelegateInfo delInfo = DelegateInfo.New<D>();
        
        if (!TryLoadInstance(method, delInfo).IsOkWithError(out var emitLoadInstance, out var error))
            return error;

        if (!TryLoadArgs(method, delInfo, emitLoadInstance.Skip).IsOkWithError(out var emitLoadArgs, out error))
            return error;

        if (!TryCall(method).IsOkWithError(out var emitCall, out error))
            return error;

        if (!TryStoreReturn(method, delInfo).IsOkWithError(out var emitReturn, out error))
            return error;

        var dm = NewDynamicILMethod<D>($"adapt<{delInfo}>{method.Render()}");
        dm.Emitter
            .Invoke(emitLoadInstance.Emit)
            .Invoke(emitLoadArgs)
            .Invoke(emitCall)
            .Invoke(emitReturn)
            .Ret();

        return dm.TryCreateDelegate();
    }
}