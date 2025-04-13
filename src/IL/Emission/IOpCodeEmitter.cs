namespace ScrubJay.Reflection.IL.Emission;

public interface IOpCodeEmitter<S> : IEmitter<S>
    where S : IOpCodeEmitter<S>
{
    /// <summary>
    /// Emits a lone <see cref="OpCode"/> onto the Stream
    /// </summary>
    /// <param name="opCode">
    /// The <see cref="OpCode"/> to emit
    /// </param>
    /// <remarks>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode)">Emit(OpCode) on learn.microsoft.com</a>
    /// </remarks>
    S Emit(OpCode opCode);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Byte">byte</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">
    /// The <see cref="OpCode"/> to emit
    /// </param>
    /// <param name="u8">
    /// The <see cref="byte"/> to emit
    /// </param>
    /// <remarks>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-byte)">Emit(OpCode, byte) on learn.microsoft.com</a>
    /// </remarks>
    S Emit(OpCode opCode, byte u8);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="SByte">sbyte</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="i8">The <see cref="sbyte"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-sbyte)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, sbyte i8);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Int16">short</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="i16">The <see cref="Int16"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-int16)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, short i16);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Int32">int</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="i32">The <see cref="Int32"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-int32)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, int i32);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Int64">long</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="i64">The <see cref="Int64"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-int64)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, long i64);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Single">float</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="f32">The <see cref="Single"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-single)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, float f32);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Double">double</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="f64">The <see cref="Double"/> to emit</param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-double)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, double f64);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="String">string</see> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="str">The <see cref="string"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-string)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, string str);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="CILLabel"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="label">The <see cref="CILLabel"/> to leave space for</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="label"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-emit-label)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, CILLabel label);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with <see cref="CILLabel"/><see cref="Array">[]</see> arguments onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="cilLabels">The <see cref="CILLabel">CILLabels</see> to leave space for</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="cilLabels"/> is empty</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="cilLabels"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-emit-label())">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, params CILLabel[] cilLabels);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="CILLocal"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="local">The <see cref="CILLocal"/> whose index to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="local"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-emit-localbuilder)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, CILLocal local);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="FieldInfo"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="field">The <see cref="FieldInfo"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="field"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-fieldinfo)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, FieldInfo field);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="ConstructorInfo"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="ctor">The <see cref="ConstructorInfo"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ctor"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-constructorinfo)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, ConstructorInfo ctor);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="MethodInfo"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="method">The <see cref="MethodInfo"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-methodinfo)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, MethodInfo method);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="Type"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="type">The <see cref="Type"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-type)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, Type type);

    /// <summary>
    /// Emits an <see cref="OpCode"/> with a <see cref="SignatureHelper"/> argument onto the Stream
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> to emit</param>
    /// <param name="signature">The <see cref="SignatureHelper"/> whose metadata token to emit</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="signature"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emit?view=net-8.0#system-reflection-emit-ilgenerator-emit(system-reflection-emit-opcode-system-reflection-emit-signaturehelper)">learn.microsoft.com</a>
    /// </links>
    S Emit(OpCode opCode, SignatureHelper signature);
}
