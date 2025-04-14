//using ScrubJay.Reflection.Collections;
//using ScrubJay.Reflection.Utilities;
//
//namespace ScrubJay.Reflection.Cloning;
//
//public static class Cloner
//{
//    [return: NotNullIfNotNull(nameof(value))]
//    internal delegate T? DeepCloneValue<T>(in T? value);
//
//    private static readonly ConcurrentTypeMap<Delegate> _deepCloneDelegateCache = [];
//
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
//
//    [return: NotNullIfNotNull(nameof(value))]
//    public static T? DeepClone<T>(in T? value)
//    {
//        if (value is null) return default(T)!;
//        var del = _deepCloneDelegateCache.GetOrAdd<T>(CreateDeepCloneDelegate);
//    }
//}