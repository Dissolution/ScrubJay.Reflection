//using ScrubJay.Reflection.Runtime;
//using ScrubJay.Reflection.Searching;
//
//namespace ScrubJay.Reflection.Cloning;
//
//public static class Cloner
//{
//    [return: NotNullIfNotNull(nameof(value))]
//    private delegate T? DeepCloneValue<T>(T? value);
//
//    private static readonly ConcurrentTypeMap<Delegate> _deepCloneDelegateCache = [];
//    private static readonly MethodInfo _clonerDeepCloneMethod;
//
//    static Cloner()
//    {
//        _clonerDeepCloneMethod = Mirror
//            .Reflect(typeof(Cloner))
//            .Methods
//            .Named(nameof(DeepClone))
//            .OneOrThrow("Could not find Cloner.DeepClone<>()");
//    }
//    
//    
//    private static DeepCloneValue<T> CreateDeepCloneUnmanagedDelegate<T>(Type type)
//    {
//        return RuntimeBuilder.EmitDelegate<DeepCloneValue<T>>(emitter => emitter.Ldarg(0).Ret()); 
//    }
//
//    private static DeepCloneValue<T> CreateDeepCloneArrayDelegate<T>(Type type)
//    {
//        Debug.Assert(type.IsArray);
//        var elementType = type.GetElementType().ThrowIfNull();
//        
//        var deepCloneElementMethod = _clonerDeepCloneMethod.MakeGenericMethod(elementType);
//        
//        var builder = RuntimeBuilder.CreateDelegateBuilder<DeepCloneValue<T>>($"{type.NameOf()}_deep_clone");
//        var emitter = builder.Emitter;
//        
//        // Get array length and store it
//        emitter
//            .DeclareLocal<int>(out var arrayLength)
//            .Ldarg(0)
//            .Ldlen()
//            .Stloc(arrayLength)
//            // Now we create a spot for the clone array
//            .DeclareLocal(type, out var clone)
//            // Get the length, create the empty array, and store in clone
//            .Ldloc(arrayLength)
//            .Newarr(elementType)
//            .Stloc(clone)
//            // int i = 0;
//            .DeclareLocal<int>(out var i)
//            .Ldc_I4_0()
//            .Stloc(i)
//            // quasi - do/while loop
//            .DefineLabel(out var lblDo)
//            .DefineLabel(out var lblWhile)
//            .Br(lblWhile)
//            .MarkLabel(lblDo)
//            // load the clone array and the index we'll store at
//            .Ldloc(clone)
//            .Ldloc(i)
//            // Load the original array, i, and then pull the element
//            .Ldarg(0)
//            .Ldloc(i)
//            .Ldelem(elementType)
//            // clone it
//            .Call(deepCloneElementMethod)
//            // store in the same position in clone
//            .Stelem(elementType)
//            // i++)
//            .Ldloc(i)
//            .Ldc_I4_1()
//            .Add()
//            .Stloc(i)
//            // i < len;
//            .MarkLabel(lblWhile)
//            .Ldloc(i)
//            .Ldloc(arrayLength)
//            .Blt(lblDo)
//            // i >= len, break out of for loop
//            // Load the clone and return it
//            .Ldloc(clone)
//            .Ret();
//
///*        var output = emitter.ToString();
//        Debugger.Break();*/
//        return builder.Build();
//    }
//    
//    private static DeepCloneValue<T> CreateComplexDeepCloneDelegate<T>(Type type)
//    {
//        var fields = Mirror.For(type).Fields.Instance.ToList();
//        if (fields.Count == 0)
//            throw new InvalidOperationException();
//
//        var builder = RuntimeBuilder.CreateDelegateBuilder<DeepCloneValue<T>>($"{type.NameOf()}_deep_clone");
//        var emitter = builder.Emitter;
//        
//        // We start by creating a local to store the clone
//        emitter.DeclareLocal(type, out var clone);
//        
//        // Init the clone
//        if (type.IsValueType)
//        {
//            // Use initobj to zero it out
//            emitter.Ldloca(clone)
//                .Initobj(type);
//        }
//        else
//        {
//            // We have a cached method to do this
//            emitter.Ldtoken(type)
//                .Call(EmissionHelper.TypeGetTypeFromHandleMethod)
//                .Call(EmissionHelper.GetUninitializedObjectMethod)
//                .Castclass(type)
//                .Stloc(clone);
//        }
//        
//        // for each field
//        foreach (var field in fields)
//        {
//            // load the clone reference (will be needed on the stack later)
//            emitter.Ldloc(clone);
//            
//            // load the field value from the source
//            emitter.Ldarg(0)
//                .Ldfld(field);
//            
//            // Deep clone it (recursive!)
//            var fieldDeepClone = _clonerDeepCloneMethod.MakeGenericMethod(field.FieldType);
//            emitter.Call(fieldDeepClone);
//            
//            // store it in the same field in clone
//            emitter.Stfld(field);
//        }
//        
//        // load the clone and return it
//        emitter.Ldloc(clone)
//            .Ret();
//
//        //var output = emitter.ToString();
//        //Debugger.Break();
//        return builder.Build();
//    }
//
//    private static DeepCloneValue<T> CreateDeepCloneDelegate<T>(Type type)
//    {
//        if (type.IsUnmanaged() || type == typeof(string))
//        {
//            return CreateDeepCloneUnmanagedDelegate<T>(type);
//        }
//        else if (type.IsArray)
//        {
//            return CreateDeepCloneArrayDelegate<T>(type);
//        }
//        else
//        {
//            return CreateComplexDeepCloneDelegate<T>(type);
//        }
//    }
//
//    [return: NotNullIfNotNull(nameof(value))]
//    public static T? DeepClone<T>(T? value)
//    {
//        if (value is null)
//            return default;
//        var del = _deepCloneDelegateCache.GetOrAdd<T>(CreateDeepCloneDelegate<T>);
//        var cloneDel = Notsafe.As<DeepCloneValue<T>>(del);
//        return cloneDel(value);
//    }
//}
