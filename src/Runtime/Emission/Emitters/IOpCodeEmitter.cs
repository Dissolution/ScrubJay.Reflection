using ScrubJay.Reflection.Runtime.Emission.Instructions;

namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

/// <summary>
/// Emits <see cref="OpCode">OpCodes</see> onto the Microsoft Intermediate Language (MSIL) <see cref="InstructionStream"/>
/// </summary>
/// <typeparam name="TEmitter"></typeparam>
public interface IOpCodeEmitter<TEmitter> : IILEmitter<TEmitter>, IILEmitter
    where TEmitter : IILEmitter<TEmitter>
{
    /// <summary>
    /// Emits a lone <see cref="OpCode"/> onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Byte">byte</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="uint8">The <see cref="byte"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-byte)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, byte uint8);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="SByte">sbyte</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="int8">The <see cref="sbyte"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-sbyte)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, sbyte int8);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Int16">short</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="int16">The <see cref="Int16"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-int16)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, short int16);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Int32">int</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="int32">The <see cref="Int32"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-int32)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, int int32);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Int64">long</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="int64">The <see cref="Int64"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-int64)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, long int64);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Single">float</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="float32">The <see cref="Single"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-single)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, float float32);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Double">double</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="float64">The <see cref="Double"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-double)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, double float64);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="String">string</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="str">The <see cref="string"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-string)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, string str);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="EmitterLabel"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="label">The <see cref="EmitterLabel"/> to leave space for</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-emit-label)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, EmitterLabel label);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with <see cref="EmitterLabel"/><see cref="Array">[]</see> arguments onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="labels">The <see cref="EmitterLabel">EmitterLabels</see> to leave space for</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="labels"/> is empty</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="labels"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-emit-label())">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, params EmitterLabel[] labels);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="EmitterLocal"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="local">The <see cref="EmitterLocal"/> whose index to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="local"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-emit-localbuilder)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, EmitterLocal local);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="FieldInfo"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="field">The <see cref="FieldInfo"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="field"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-fieldinfo)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, FieldInfo field);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="ConstructorInfo"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="ctor">The <see cref="ConstructorInfo"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ctor"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-constructorinfo)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, ConstructorInfo ctor);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="MethodInfo"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="method">The <see cref="MethodInfo"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-methodinfo)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, MethodInfo method);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Type"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="type">The <see cref="Type"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-type)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, Type type);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="SignatureHelper"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="signature">The <see cref="SignatureHelper"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="signature"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-emit-signaturehelper)">learn.microsoft.com</a>
    /// </links>
    TEmitter Emit(OpCode opCode, SignatureHelper signature);
}