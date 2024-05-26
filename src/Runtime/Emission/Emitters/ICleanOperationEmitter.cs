// ReSharper disable IdentifierTypo
namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface ICleanOperationEmitter<TEmitter> : IOperationEmitter<TEmitter> 
    where TEmitter : IILEmitter<TEmitter>
{
    TEmitter Ldarg(int index);
    TEmitter Ldarga(int index);
    
    TEmitter LoadArgs(Range range);
    TEmitter LoadArgs(params int[] indices);
    
    TEmitter Starg(int index);

    TEmitter Ldloc(int index);
    TEmitter Ldloc(EmitterLocal local);
    TEmitter Ldloca(int index);
    TEmitter Ldloca(EmitterLocal local);
    
    TEmitter Stloc(int index);
    TEmitter Stloc(EmitterLocal local);

    TEmitter Call(MethodBase method);

#region Math
    TEmitter Add(bool unsigned = false, bool checkOverflow = false);
    TEmitter Div(bool unsigned = false);
    TEmitter Mul(bool unsigned = false, bool checkOverflow = false);
    TEmitter Rem(bool unsigned = false);
    TEmitter Sub(bool unsigned = false, bool checkOverflow = false);
#endregion

#region Bitwise
    TEmitter Shr(bool unsigned = false);
#endregion

#region Branching
    TEmitter Br(EmitterLabel label);
    TEmitter Leave(EmitterLabel label);

    TEmitter Brtrue(EmitterLabel label);
    TEmitter Brfalse(EmitterLabel label);
    TEmitter Beq(EmitterLabel label);
    TEmitter Bne(EmitterLabel label);
    TEmitter Bge(EmitterLabel label, bool unsigned = false);
    TEmitter Bgt(EmitterLabel label, bool unsigned = false);
    TEmitter Ble(EmitterLabel label, bool unsigned = false);
    TEmitter Blt(EmitterLabel label, bool unsigned = false);
#endregion

#region Conv
    TEmitter Conv(Type type, bool unsigned = false, bool checkOverflow = false);
    TEmitter Conv<T>(bool unsigned = false, bool checkOverflow = false);
#endregion

#region Comparison
    TEmitter Cgt(bool unsigned = false);
    TEmitter Clt(bool unsigned = false);
#endregion
    
#region Load Value
    TEmitter Ldc_I4(int value);
    TEmitter Ldc_I8(long value);
    TEmitter Ldc_R4(float value);
    TEmitter Ldc_R8(double value);
#endregion

#region Arrays
    TEmitter Ldelem(Type type);
    TEmitter Ldelem<T>();
    TEmitter Ldelema(Type type);
    TEmitter Ldelema<T>();
    
    TEmitter Stelem(Type type);
    TEmitter Stelem<T>();
#endregion

#region Fields
    TEmitter Ldfld(FieldInfo field);
    TEmitter Ldflda(FieldInfo field);
    
    TEmitter Stfld(FieldInfo field);
#endregion

#region Load / Store via Address
    TEmitter Ldind(Type type);
    TEmitter Ldind<T>();
    
    TEmitter Stind(Type type);
    TEmitter Stind<T>();
#endregion
}

