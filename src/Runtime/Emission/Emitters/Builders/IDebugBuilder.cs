using System.Diagnostics;
using ScrubJay.Reflection.Searching;

namespace ScrubJay.Reflection.Runtime.Emission.Emitters.Builders;

public interface IDebugBuilder<out TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter Break();
    
    TEmitter NoOperation();

    TEmitter WriteLine(string str);
    TEmitter WriteLine(FieldInfo field);
    TEmitter WriteLine(EmitterLocal local);
}

internal static class DebugBuilder
{
    public static MethodInfo Debug_WriteLine_String_Method { get; }

    static DebugBuilder()
    {
        Debug_WriteLine_String_Method = Mirror
            .TryFindMember<MethodInfo>(() => Debug.WriteLine("ABC"))
            .OkOrThrow();
    }

    public static MethodInfo? FindWriteLine(Type type)
    {
        return Mirror.Search(typeof(Debug))
            .TryFindMember<MethodInfo>(b => b.Public.Static.Method.Name("WriteLine").ReturnsVoid().ParameterTypes(type))
            .OkOrFallback(null!);
    }

    public static MethodInfo FindToString(Type type)
    {
        return Mirror.Search(type)
            .TryFindMember<MethodInfo>(b => b.Public.Instance.Method.Name(nameof(ToString)).NoParameters().ReturnType(typeof(string)))
            .OkOrThrow();
    }
}

internal class DebugBuilder<TEmitter> : BuilderBase<TEmitter>, IDebugBuilder<TEmitter>
    where TEmitter : ICleanEmitter<TEmitter>
{
    
    public DebugBuilder(Emitter<TEmitter> emitter) : base(emitter)
    {
    }
    
    public TEmitter Break() => _emitter.Break();
    public TEmitter NoOperation() => _emitter.Nop();

    public TEmitter WriteLine(string str)
    {
        return _emitter.Ldstr(str)
            .Call(DebugBuilder.Debug_WriteLine_String_Method);
    }

    public TEmitter WriteLine(FieldInfo field)
    {
        if (field.IsStatic)
        {
            _emitter.Ldsfld(field);
        }
        else
        {
            // Have to assume we're in an instance method
            _emitter.Ldarg(0)
                .Ldfld(field);
        }

        var writeLineMethod = DebugBuilder.FindWriteLine(field.FieldType);
        
        if (writeLineMethod is not null)
        {
            return _emitter.Call(writeLineMethod);
        }
        else
        {
            return _emitter.Call(DebugBuilder.FindToString(field.FieldType))
                .Call(DebugBuilder.Debug_WriteLine_String_Method);
        }
    }

    public TEmitter WriteLine(EmitterLocal local)
    {
        _emitter.Ldloc(local);
        
        var writeLineMethod = DebugBuilder.FindWriteLine(local.LocalType);
        
        if (writeLineMethod is not null)
        {
            return _emitter.Call(writeLineMethod);
        }
        else
        {
            return _emitter.Call(DebugBuilder.FindToString(local.LocalType))
                .Call(DebugBuilder.Debug_WriteLine_String_Method);
        }
    }
}