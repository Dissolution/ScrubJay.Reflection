using Dunet;
using ScrubJay.Reflection.Extensions;
using ScrubJay.Reflection.Info;
using ScrubJay.Validation;

namespace ScrubJay.Reflection.Runtime.Emission.Adaption;

[Union]
public abstract partial record class Variable
{
    public required Type Type { get; init; }
    
    public required bool CanLoad { get; init; }
    public required bool CanLoadAddress { get; init; }
    public required bool CanStore { get; init; }

    public abstract TEmitter EmitLoad<TEmitter>(TEmitter emitter)
        where TEmitter : IOpCodeEmitter<TEmitter>;
    public abstract TEmitter EmitLoadAddress<TEmitter>(TEmitter emitter)
        where TEmitter : IOpCodeEmitter<TEmitter>;
    public abstract TEmitter EmitStore<TEmitter>(TEmitter emitter)
        where TEmitter : IOpCodeEmitter<TEmitter>;

    
    
    public partial record Stack : Variable
    {
        [SetsRequiredMembers]
        public Stack(Type type)
        {
            this.CanLoad = true;
            this.CanStore = true;
            this.Type = type;
        }

        public override TEmitter EmitLoad<TEmitter>(TEmitter emitter) => emitter;
        public override TEmitter EmitLoadAddress<TEmitter>(TEmitter emitter) => emitter;
        public override TEmitter EmitStore<TEmitter>(TEmitter emitter) => emitter;
    }
    
    
    
    public partial record Field : Variable
    {
        [SetsRequiredMembers]
        public Field(FieldInfo field)
        {
            this.CanLoad = true;
            this.CanLoadAddress = true;
            this.CanStore = true;
            this.Type = field.FieldType;
        }

        public override TEmitter EmitLoad<TEmitter>(TEmitter emitter) => emitter.Emit(OpCodes.Ldfld);
        public override TEmitter EmitLoadAddress<TEmitter>(TEmitter emitter) => emitter.Emit(OpCodes.Ldflda);
        public override TEmitter EmitStore<TEmitter>(TEmitter emitter) => emitter.Emit(OpCodes.Stfld);
    }

    public partial record InstanceField : Variable
    {
        public required Instance Instance { get; init; }
        public required Field Field { get; init; }

        [SetsRequiredMembers]
        public InstanceField(Instance instance, Field field)
        {
            this.Instance = instance;
            this.Field = field;
            
            this.CanLoad = field.CanLoad;
            this.CanLoadAddress = field.CanLoadAddress;
            this.CanStore = field.CanStore;
            this.Type = field.Type;
        }
        
        public override TEmitter EmitLoad<TEmitter>(TEmitter emitter)
        {
            Instance.EmitLoadAddress(emitter);
            Field.EmitLoad(emitter);
            return emitter;
        }
        public override TEmitter EmitLoadAddress<TEmitter>(TEmitter emitter)
        {
            Instance.EmitLoadAddress(emitter);
            Field.EmitLoadAddress(emitter);
            return emitter;
        }
        public override TEmitter EmitStore<TEmitter>(TEmitter emitter)
        {
            Instance.EmitLoadAddress(emitter);
            Field.EmitStore(emitter);
            return emitter;
        }
    }

    public partial record Instance : Variable
    {
        public int ArgIndex { get; }
        
        [SetsRequiredMembers]
        public Instance(ParameterInfo parameter)
        {
            this.CanLoad = true;
            this.CanLoadAddress = true;
            this.CanStore = false;
            this.ArgIndex = parameter.Position;
            this.Type = parameter.ParameterType;
        }

        public override TEmitter EmitLoad<TEmitter>(TEmitter emitter) => emitter.Emit(OpCodes.Ldarg, ArgIndex);
        public override TEmitter EmitLoadAddress<TEmitter>(TEmitter emitter) => emitter.Emit(OpCodes.Ldarga, ArgIndex);
        public override TEmitter EmitStore<TEmitter>(TEmitter emitter) => throw new InvalidOperationException();
    }
}



//
//[Union]
//public abstract partial record class Variable
//{
//    partial record Parameter(ParameterInfo ParameterInfo)
//    {
//        public override Type Type => this.ParameterInfo.ParameterType;
//        public override RefKind RefKind => this.ParameterInfo.RefKind();
//    }
//
//    partial record Stack(Type StackType)
//    {
//        public override Type Type => this.StackType;
//        public override RefKind PreferredRefKind => this.RefKind;
//    }
//
//    partial record Local(EmitterLocal EmitterLocal)
//    {
//        public override Type Type => EmitterLocal.Type;
//    }
//    
//    public abstract Type Type { get; }
//    public virtual RefKind RefKind => Type.RefKind();
//    public virtual Type NonRefType => Type.IsByRef ? Type.GetElementType().ThrowIfNull() : Type;
//    public virtual RefKind PreferredRefKind => RefKind.None | RefKind.Ref; // either
//}
//
//
//public static class VariableHelper
//{
//    public static Result<(ConvertComplexity Complexity, Action<TEmitter> EmitConvert), Exception> TryGetConverter<TEmitter>(Variable sourceVar, Variable destVar)
//    {
//        ConvertComplexity complexity = default;
//        
//        // SourceType ~~ DestType
//        if (sourceVar.NonRefType == destVar.NonRefType)
//        {
//            // 1:1
//            if (sourceVar.Type == destVar.Type)
//            {
//                // Do we need to load?
//                if (sourceVar.Match())
//            }
//            
//        }
//    
//
//
//        InvalidOperationException GetEx() => new(
//            $"Cannot load {sourceVar} as {destVar}",
//            new NotImplementedException("This may be implemented in the future"))
//        {
//            Data =
//            {
//                ["SourceVar"] = sourceVar,
//                ["DestVar"] = destVar,
//            },
//        };
//    }
//}
//
//[Flags]
//public enum ConvertComplexity
//{
//    
//}