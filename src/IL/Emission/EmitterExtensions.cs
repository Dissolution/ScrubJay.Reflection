
namespace ScrubJay.Reflection.IL.Emission;

public static class EmitterExtensions
{
    /*
    public static E EmitCast<E>(this E emitter, Arg source, Arg dest)
    {
        source.TryLoadAs(emitter, dest).ThrowIfFailed();
        return emitter;
    }

    public static IFluentILEmitter EmitLoadParameter(this IFluentILEmitter emitter, Arg sourceParameter, Arg destType)
    {
        sourceParameter.TryLoadAs(emitter, destType).ThrowIfFailed();
        return emitter;
    }

    public static IFluentILEmitter EmitLoadInstance(this IFluentILEmitter emitter, Arg instance, MemberInfo member)
    {
        // No instance, nothing to load
        if (!member.TryGetInstanceType(out var instanceType)) return emitter;
        instance.TryLoadAs(emitter, instanceType).ThrowIfFailed();
        return emitter;
    }
    
    public static IFluentILEmitter EmitParamsLengthCheck(this IFluentILEmitter emitter,
        ParameterInfo paramsParameter, int length)
    {
        return emitter
            .Ldarg(paramsParameter)
            .Ldlen()
            .Ldc_I4(length)
            .Beq(out var lenEqual)
            .Ldstr($"{length} parameters are required in the params array")
            .Ldstr(paramsParameter.Name)
            .Newobj(Searching.MemberSearch.FindConstructor<ArgumentException>(typeof(string), typeof(string)))
            .Throw()
            .MarkLabel(lenEqual);
    }

    public static IFluentILEmitter EmitLoadParams(this IFluentILEmitter emitter,
        ParameterInfo paramsParameter,
        ReadOnlySpan<ParameterInfo> destParameters)
    {
        int len = destParameters.Length;
        // None to load?
        if (len == 0)
            return emitter;

        // Params -> Params?
        if (len == 1 && destParameters[0].IsParams())
        {
            emitter.Ldarg(paramsParameter);
        }
        else
        {
            // extract each parameter in turn
            for (var i = 0; i < len; i++)
            {
                emitter.Ldarg(paramsParameter)
                    .Ldc_I4(i)
                    .Ldelem(destParameters[i].ParameterType);
            }
        }

        // Everything will be loaded!
        return emitter;
    }
    */

    public static E EmitThrowException<E, X>(this E emitter, params object?[] exceptionArgs)
        where E : ISimpleEmitter<E>
        where X : Exception
    {
        var exCtor = Reflect<X>()
            .Instance
            .Constructors()
            .Accepting(exceptionArgs)
            .OneOrThrow();
//            
//        
//        
//        var exArgsTypes = exceptionArgs.Select(arg => arg?.GetType()).ToList();
//
//        // Find the ctor we can call with these args
//        var validCtors = typeof(E, X)
//            .GetConstructors(Reflect.Flags.Instance)
//            .Where(ctor =>
//            {
//                var ctorParams = ctor.GetParameters();
//                int ctorParamsCount = ctorParams.Length;
//                if (ctorParamsCount != exArgsTypes.Count) return false;
//                var impl = true;
//                for (var i = 0; i < ctorParamsCount; i++)
//                {
//                    var argType = exArgsTypes[i]?.GetType();
//                    var paramType = ctorParams[i].ParameterType;
//                    if (!argType.Implements(paramType)) return false;
//                }
//                return true;
//            })
//            .ToList();
//        Debugger.Break();
//        var ctor = validCtors[0];
//        foreach (var arg in exceptionArgs)
//        {
//            emitter.LoadValue(arg);
//        }
//        emitter.Newobj(ctor).Throw();
//
//        var il = emitter.ToString();
        Debugger.Break();
        
        return emitter;
    }
}