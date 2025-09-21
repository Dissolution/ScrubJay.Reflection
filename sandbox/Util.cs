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
        [NotNull, DisallowNull]
        public int Int_NN_DN { get; set; }
        
        [NotNull, AllowNull]
        public int Int_NN_AN { get; set; }
        
        [MaybeNull, DisallowNull]
        public int Int_MN_DN { get; set; }
        
        [MaybeNull, AllowNull]
        public int Int_MN_AN { get; set; }
        
        [NotNull, DisallowNull]
        public int? NInt_NN_DN { get; set; }
        
        [NotNull, AllowNull]
        public int? NInt_NN_AN { get; set; }
        
        [MaybeNull, DisallowNull]
        public int? NInt_MN_DN { get; set; }
        
        [MaybeNull, AllowNull]
        public int? NInt_MN_AN { get; set; }
        
        
        [NotNull, DisallowNull]
        public object Obj_NN_DN { get; set; }
        
        [NotNull, AllowNull]
        public object Obj_NN_AN { get; set; }
        
        [MaybeNull, DisallowNull]
        public object Obj_MN_DN { get; set; }
        
        [MaybeNull, AllowNull]
        public object Obj_MN_AN { get; set; }
        
        
        [NotNull, DisallowNull]
        public object? NObj_NN_DN { get; set; }
        
        [NotNull, AllowNull]
        public object? NObj_NN_AN { get; set; }
        
        [MaybeNull, DisallowNull]
        public object? NObj_MN_DN { get; set; }
        
        [MaybeNull, AllowNull]
        public object? NObj_MN_AN { get; set; }
        
        public Guid G_GI { get; init; }
    }
}