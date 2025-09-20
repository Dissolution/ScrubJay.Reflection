using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using ScrubJay.Functional;
// ReSharper disable UseNullableAnnotationInsteadOfAttribute
// ReSharper disable IdentifierTypo
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace ScrubJay.Reflection.Sandbox;

public static class Util
{
    public static HashSet<Type> GetAllTypes()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(static ass => Result.Try(ass.GetTypes).OkOr([]))
            .ToHashSet();
    }

    public static bool IsLoc(OpCode opCode)
    {
        return MemoryExtensions.Contains(opCode.Name.AsSpan(), "loc".AsSpan(), StringComparison.OrdinalIgnoreCase);
    }
    
    
    
    public class NullabilityProperties
    {
        public object Prop { get; set; }
        
        [AllowNull]
        public object AllowNullProp { get; set; }
        
        [DisallowNull]
        public object DisallowNullProp { get; set; }
        
        
        [NotNull]
        public object PropNotNull { get; set; }
        
        [MaybeNull]
        public object PropMaybeNull { get; set; }
        
        
        [AllowNull, NotNull]
        public object AllowNullPropNotNull { get; set; }
        
        [AllowNull, MaybeNull]
        public object AllowNullPropMaybeNull { get; set; }
        
        
        [DisallowNull, NotNull]
        public object DisallowNullPropNotNull { get; set; }
        
        [DisallowNull, MaybeNull]
        public object DisallowNullPropMaybeNull { get; set; }
        
        
        
        public object? PropQ { get; set; }
        
        [AllowNull]
        public object? AllowNullPropQ { get; set; }
        
        [DisallowNull]
        public object? DisallowNullPropQ { get; set; }
        
        
        [NotNull]
        public object? PropQNotNull { get; set; }
        
        [MaybeNull]
        public object? PropQMaybeNull { get; set; }
        
        
        [AllowNull, NotNull]
        public object? AllowNullPropQNotNull { get; set; }
        
        [AllowNull, MaybeNull]
        public object? AllowNullPropQMaybeNull { get; set; }
        
        
        [DisallowNull, NotNull]
        public object? DisallowNullPropQNotNull { get; set; }
        
        [DisallowNull, MaybeNull]
        public object? DisallowNullPropQMaybeNull { get; set; }
    }
}