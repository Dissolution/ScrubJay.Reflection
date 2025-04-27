using ScrubJay.Reflection.Adapting.Arguments;
using ScrubJay.Reflection.IL.Emission;
using ScrubJay.Reflection.MosDef;
using ScrubJay.Reflection.Naming;

using Emit = System.Action<ScrubJay.Reflection.IL.Emission.Emitter>;

namespace ScrubJay.Reflection.Adapting;

public class MethodDelegateAdapter<D> : MemberDelegateAdapter<MethodDelegateAdapter<D>, MethodBase, D>
    where D : Delegate
{
    public override Result<D> TryAdapt(MethodBase? method)
    {
        if (method is null)
            return GetEx(method);

        //DelegateInfo delInfo = DelegateInfo.Create<D>();


        DynamicILMethod<D> dm = RuntimeBuilder.BuildDynamicMethod<D>($"{typeof(D).NameOf()} wraps {method.NameOf()}");

        if (!TryLoadInstance(dm, method).IsOkWithError(out var emitLoadInstance, out var error))
            return error;

        if (!TryLoadArgs(dm, method, emitLoadInstance.Skip).IsOkWithError(out var emitLoadArgs, out error))
            return error;

        if (!TryCall(dm, method).IsOkWithError(out var emitCall, out error))
            return error;

        if (!TryStoreReturn(dm, method).IsOkWithError(out var emitReturn, out error))
            return error;

        dm.Emitter
            .Invoke(emitLoadInstance.Emit)
            .Invoke(emitLoadArgs)
            .Invoke(emitCall)
            .Invoke(emitReturn)
            .Ret();

        string str = dm.ToString();
        Debug.WriteLine(str);
        
        return dm.TryCreateDelegate();
    }



    private Result<(bool Skip, Emit Emit)> TryLoadInstance(DynamicILMethod<D> builder, MethodBase method)
    {
        // static?
        if (method.IsStatic)
        {
            if (builder.ParameterCount == 0)
            {
                // do nothing
                return Ok(false);
            }
            
            // check if the first param is something we can skip
            var firstParam = builder.Parameters[0];

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
            if (builder.ParameterCount == 0)
            {
                return GetEx(method, "Delegate does not have required instance parameter");
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
            
            var firstParam = builder.Parameters[0];

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

            return GetEx(method, "Delegate Instance parameter cannot be adapter to method instance");
        }

        
        static Result<(bool,Emit)> Ok(bool skip, Emit? emit = null)
        {
            return Ok<(bool, Emit)>((skip, emit ?? (_ => { })));
        }
    }

    private Result<Emit> TryLoadArgs(DynamicILMethod<D> builder, MethodBase method, bool skip)
    {
        ReadOnlySpan<ParameterInfo> builderParameters = builder.Parameters;
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
            
            var result = ArgumentConverter.CanConvert(builderParam, new StackArgument(methodParam.ParameterType));
            if (!result.IsOkWithError(out var paramEmit, out var error))
                return error;

            emissions.Add(paramEmit);

            i++;
        }
        
        
        
        
        methodExcess:
        delegateExcess:
        throw new NotImplementedException();
    }

    private Result<Emit> TryCall(DynamicILMethod<D> builder, MethodBase method)
    {
        if (method is ConstructorInfo ctor)
            return Ok<Emit>(emitter => emitter.Newobj(ctor));
        if (method is MethodInfo meth)
            return Ok<Emit>(emitter => emitter.Call(meth));
        return new ArgumentException(null, nameof(method));
    }
    
    private Result<Emit> TryStoreReturn(DynamicILMethod<D> builder, MethodBase method)
    {
        var methodReturn = method.ReturnType();

        var delegateReturn = builder.ReturnType;

        var result = ArgumentConverter.CanConvert(methodReturn, delegateReturn);
        if (!result.IsOkWithError(out var emit, out var error))
            return error;

        return Ok<Emit>(emit);
    }
}