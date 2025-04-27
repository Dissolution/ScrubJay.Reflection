using System.CodeDom.Compiler;
using ScrubJay.Debugging;
using ScrubJay.Reflection.IL.Emission;
using System.Text.Json;
using ScrubJay.Collections.NonGeneric;
using ScrubJay.Reflection.Collections;
using ScrubJay.Reflection.Utilities;
using static InlineIL.IL;

namespace ScrubJay.Reflection.Cloning;

public static class DeepJson
{
    private const string INDENT = "    ";
    private static readonly DelegateMap _cache = new();

    private static bool IsNumber(object obj)
    {
        return obj is
            byte or sbyte or short or ushort or int or uint or long or ulong or nint or nuint or IntPtr or UIntPtr;
    }

    private static bool IsFloatingPoint(object obj)
    {
        return obj is
            float or double or decimal;
    }


    private static IndentTextBuilder JsonAppendArray(this IndentTextBuilder builder, Array array)
    {
        var elementType = array.GetType().GetElementType().ThrowIfNull();

        Action<IndentTextBuilder> delimit;
        if (elementType.IsPrimitive)
        {
            delimit = static tb => tb.Append(", ");
        }
        else
        {
            delimit = static tb => tb.Append(',').NewLine();
        }
        
        builder.Append('[')
            .EnumerateAndDelimit(array.OfType<object?>(),
                static (tb, item) => tb.JsonAppendNameValue(null, item),
                delimit)
            .Append(']');
        
        return builder;
    }
    
    
    private static IndentTextBuilder JsonAppendComplex(this IndentTextBuilder builder, object value)
    {
        builder.Append('{').AddIndent(INDENT).NewLine();

        Type valueType = value.GetType();
        
        var fields = Reflect(valueType)
            .Instance.Fields()
            .AsList();

        if (fields.Count == 0)
        {
            // Special case
            builder.JsonAppendNameValue("Type", valueType);
        }
        else
        {
            builder.EnumerateAndDelimit(fields,
                (tb, field) =>
                {
                    object? fieldValue = field.GetValue(value);
                    tb.JsonAppendNameValue(field.Name, fieldValue);
                },
                tb => tb.Append(',').NewLine());
        }

        builder
            .RemoveIndent()
            .NewLine()
            .Append('}');
        return builder;
    }
    
    private static IndentTextBuilder JsonAppendNameValue(this IndentTextBuilder builder,
        string? name, object? value)
    {
        builder.IfNotEmpty(name, static (tb, n) => tb.Append('"').Append(n).Append("\": "));
        if (value is null)
        {
            return builder.Append("null");
        }
        else if (value is string str)
        {
            return builder.Append('"').Append(str).Append('"');
        }
        else if (value is bool boolean)
        {
            return builder.Append(boolean ? "true" : "false");
        }
        else if (IsNumber(value))
        {
            return builder.Append(value, "D");
        }
        else if (IsFloatingPoint(value))
        {
            return builder.Append(value, "F");
        }
        else if (value is Guid guid)
        {
            return builder.Append('"').Append(value, "D").Append('"');
        }
        else if (value is Array array)
        {
            return JsonAppendArray(builder, array);
        }
        else if (value is Type type)
        {
            return builder.Append('"').AppendType(type).Append('"');
        }
        else
        {
            Type valueType = value.GetType();
            if (valueType.IsPrimitive || valueType.IsUnmanaged())
                Debugger.Break();
            return JsonAppendComplex(builder, value);
        }
    }

    public static string Render<T>(T? value)
    {
        return IndentTextBuilder.New
            .JsonAppendNameValue(null, value)
            .ToStringAndDispose();
    }
}

//   
//
//    private static readonly MethodInfo _deepCloneUnmanagedMethod = Reflect(typeof(Cloner))
//        .Private.Static.Methods().Named(nameof(DeepCloneUnmanaged))
//        .GenericCount(1)
//        .OneOrThrow();
//
//    private static readonly MethodInfo _deepCloneMethod = Reflect(typeof(Cloner))
//        .Public.Static.Methods().Named(nameof(DeepClone))
//        .GenericCount(1)
//        .OneOrThrow();
//    
//    private static U DeepCloneUnmanaged<U>(U value)
//        where U : unmanaged
//    {
//        Emit.Ldarga(nameof(value));
//        Emit.Ldobj<U>();
//        return Return<U>();
//    }
//
//    private static Delegate DeepCloneArray(Type arrayType)
//    {
//        Debug.Assert(arrayType.IsArray);
//        var elementType = arrayType.GetElementType();
//        Debug.Assert(elementType is not null);
//        
//        var deepCloneItemMethod = _deepCloneMethod.MakeGenericMethod(elementType);
//
//        var methodBuilder =RuntimeBuilder.BuildDynamicMethod(
//            typeof(DeepCloneValue<>).MakeGenericType(arrayType));
//        methodBuilder.Emitter
//        // load the array's length and store it
//            .DeclareLocal<int>(out var arrayLen)
//            .Ldarg_0()
//            .Ldlen()
//            .Stloc(arrayLen)
//            // Create the cloned array local
//            .DeclareLocal(arrayType, out var clone)
//            // use the length to create an empty array and store it in clone
//            .Ldloc(arrayLen)
//            .Newarr(elementType!)
//            .Stloc(clone)
//            // int i = 0;
//            .DeclareLocal<int>(out var i)
//            .Ldc_I4_0()
//            .Stloc(i)
//            // define do and while labels, then branch to the while check
//            .DefineLabel(out var lblDo)
//            .DefineLabel(out var lblWhile)
//            .Br(lblWhile)
//            .MarkLabel(lblDo)
//            // inside do
//            // load the clone array and the index in the array we'll be storing at (sets up stack for later)
//            .Ldloc(clone)
//            .Ldloc(i)
//            // load the original array, index
//            .Ldarg(0)
//            .Ldloc(i)
//            // load the original's item, copy it, and store in clone
//            .Ldelem(elementType!)
//            .Call(deepCloneItemMethod)
//            .Stelem(elementType!)
//            // increment i
//            .Ldloc(i)
//            .Ldc_I4_1()
//            .Add()
//            .Stloc(i)
//            // while (i < arrayLen)
//            .MarkLabel(lblWhile)
//            .Ldloc(i)
//            .Ldloc(arrayLen)
//            .Blt(lblDo)
//            // we have finished, load and return the clone
//            .Ldloc(clone)
//            .Ret();
//        
//        return methodBuilder.TryCreateDelegate().OkOrThrow();
//    }
//    
//    private static Delegate /*DeepCloneValue<T>*/ CreateDeepCloneDelegate(Type type)
//    {
//        if (type.IsUnmanaged())
//        {
//            return Delegate.CreateDelegate(
//                typeof(DeepCloneValue<>).MakeGenericType(type),
//                _deepCloneUnmanagedMethod.MakeGenericMethod(type));
//        }
//        else if (type == typeof(string))
//        {
//            Debugger.Break();
//            return DeepCloneString;
//        }
//        
//        Debug.Assert(!type.IsPrimitive);
//
//        if (type.IsArray)
//            return DeepCloneArray(type);
//
//        var fields = Reflect(type).Instance.Fields().AsList();
//
//        if (fields.Count == 0)
//            throw new NotImplementedException();
//        
//        var methodBuilder = new DynamicMethodBuilder(typeof(DeepCloneValue<>).MakeGenericType(type));
//        var emitter = methodBuilder.Emitter;
//        
//        // declare the clone
//        emitter.DeclareLocal(type, out var clone);
//        
//        // init the clone so it's not null
//        if (type.IsValueType)
//        {
//            emitter.Ldloca(clone)
//                .Initobj(type)
//                .Nop();
//        }
//        else
//        {
//            emitter.Ldtoken(type)
//                .Call(EmissionHelper.Type_GetTypeFromHandle_Method)
//                .Call(EmissionHelper.GetUninitializedObject_Method)
//                .Castclass(type)
//                .Stloc(clone)
//                .Nop();
//        }
//        
//        
//        foreach (var field in fields)
//        {
//            if (type.IsValueType)
//            {
//                emitter.Ldloca(clone);
//            }
//            else
//            {
//                emitter.Ldloc(clone);
//            }
//            
//            emitter
//                .Ldarg(0)
//                .Ldfld(field)
//                .Call(_deepCloneMethod.MakeGenericMethod(field.FieldType))
//                .Stfld(field)
//                .Nop();
//        }
//
//        emitter.Ldloc(clone)
//            .Ret();
//
//        return methodBuilder.TryCreateDelegate().OkOrThrow();
//    }
//
//
//    [return: NotNullIfNotNull(nameof(value))]
//    public static T? DeepClone<T>(this T? value)
//    {
//        if (value is null)
//            return default(T)!;
//        
//        try
//        {
//            var del = _deepCloneDelegateCache.GetOrAdd<T>(static type => CreateDeepCloneDelegate(type));
//            var deepCloner = del as DeepCloneValue<T>;
//       
//            var clone = deepCloner!(value);
//            return clone;
//        }
//        catch (Exception ex)
//        {
//            var dump = ex.Dump();
//            Console.WriteLine(dump);
//            Debugger.Break();
//            throw;
//        }
//       
//    }
//}