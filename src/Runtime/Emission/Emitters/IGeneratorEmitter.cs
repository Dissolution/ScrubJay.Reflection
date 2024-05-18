namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEmitter"></typeparam>
/// <links>
/// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator?view=net-8.0#methods">learn.microsoft.com</a>
/// </links>
public interface IGeneratorEmitter<TEmitter> : IILEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
#region try/catch/finally
    /// <summary>
    /// Begins an exception block for a non-filtered <see cref="Exception"/>
    /// </summary>
    /// <param name="label">
    /// The <see cref="EmitterLabel"/> that points to the end of the <c>try/catch</c> block<br/>
    /// This will leave you in the correct place to execute <see langword="finally"/> blocks or to finish the <see langword="try"/>.
    /// </param>
    /// <param name="labelName">Automatically captured name for the <see cref="EmitterLabel"/></param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptionblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter BeginExceptionBlock(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    /// <summary>
    /// Begins a <c>catch</c> block
    /// </summary>
    /// <param name="exceptionType">The <see cref="Type"/> of <see cref="Exception"/> to catch</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is <c>null</c></exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="exceptionType"/> is not a valid <see cref="Exception"/> <see cref="Type"/></exception>
    /// <exception cref="ArgumentException">Thrown if within a filtered exception</exception>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter BeginCatchBlock(Type exceptionType);

    /// <summary>
    /// Begins a <c>catch</c> block
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/> to catch</typeparam>
    /// <exception cref="ArgumentException">Thrown if within a filtered exception</exception>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter BeginCatchBlock<TException>()
        where TException : Exception;

    /// <summary>
    /// Begins a <see langword="finally"/> block
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfinallyblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter BeginFinallyBlock();

    /// <summary>
    /// Begins an exception block for a filtered <see cref="Exception"/>
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptfilterblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter BeginExceptFilterBlock();

    /// <summary>
    /// Begins an exception fault block
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfaultblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter BeginFaultBlock();

    /// <summary>
    /// Ends an exception block
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if this operation occurs in an unexpected place</exception>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endexceptionblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter EndExceptionBlock();
#endregion
#region Scope
    /// <summary>
    /// Begins a lexical scope
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginscope?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter BeginScope();

    /// <summary>
    /// Ends a lexical scope
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endscope?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter EndScope();

    /// <summary>
    /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope
    /// </summary>
    /// <param name="namespace">The namespace to be used in evaluating locals and watches for the current active lexical scope</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="namespace"/> is <c>null</c> or has a <see cref="string.Length"/> of 0</exception>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.usingnamespace?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter UsingNamespace(string @namespace);
#endregion
#region Locals
    /// <summary>
    /// Declares a <see cref="EmitterLocal"/> variable
    /// </summary>
    /// <param name="localType">The type of the <see cref="EmitterLocal"/></param>
    /// <param name="local">Outputs the declared <see cref="EmitterLocal"/></param>
    /// <param name="localName">Automatically captured name for the <see cref="EmitterLocal"/></param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="localType"/> is <c>null</c></exception>
    /// <exception cref="InvalidOperationException">Thrown if within <see cref="TypeBuilder.CreateType"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=net-8.0#system-reflection-emit-ilgenerator-declarelocal(system-type)">learn.microsoft.com</a>
    /// </links>
    TEmitter DeclareLocal(Type localType, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);

    /// <summary>
    /// Declares a <see cref="EmitterLocal"/> variable
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the <see cref="EmitterLocal"/></typeparam>
    /// <param name="local">Outputs the declared <see cref="EmitterLocal"/></param>
    /// <param name="localName">Automatically captured name for the <see cref="EmitterLocal"/></param>
    /// <exception cref="InvalidOperationException">Thrown if within <see cref="TypeBuilder.CreateType"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=net-8.0#system-reflection-emit-ilgenerator-declarelocal(system-type)">learn.microsoft.com</a>
    /// </links>
    TEmitter DeclareLocal<T>(out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);

    /// <summary>
    /// Declares a <see cref="EmitterLocal"/> variable
    /// </summary>
    /// <param name="localType">The type of the <see cref="EmitterLocal"/></param>
    /// <param name="pinned">Whether or not the <see cref="EmitterLocal"/> should be pinned in memory</param>
    /// <param name="local">Outputs the declared <see cref="EmitterLocal"/></param>
    /// <param name="localName">Automatically captured name for the <see cref="EmitterLocal"/></param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="localType"/> is <c>null</c></exception>
    /// <exception cref="InvalidOperationException">Thrown if within <see cref="TypeBuilder.CreateType"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=net-8.0#system-reflection-emit-ilgenerator-declarelocal(system-type-system-boolean)">learn.microsoft.com</a>
    /// </links>
    TEmitter DeclareLocal(Type localType, bool pinned, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);

    /// <summary>
    /// Declares a <see cref="EmitterLocal"/> variable
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the <see cref="EmitterLocal"/></typeparam>
    /// <param name="pinned">Whether or not the <see cref="EmitterLocal"/> should be pinned in memory</param>
    /// <param name="local">Outputs the declared <see cref="EmitterLocal"/></param>
    /// <param name="localName">Automatically captured name for the <see cref="EmitterLocal"/></param>
    /// <exception cref="InvalidOperationException">Thrown if within <see cref="TypeBuilder.CreateType"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=net-8.0#system-reflection-emit-ilgenerator-declarelocal(system-type-system-boolean)">learn.microsoft.com</a>
    /// </links>
    TEmitter DeclareLocal<T>(bool pinned, out EmitterLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);
#endregion
#region Labels
    /// <summary>
    /// Declares a new <see cref="EmitterLabel"/>
    /// </summary>
    /// <param name="label">Outputs the declared <see cref="EmitterLabel"/></param>
    /// <param name="labelName">Automatically captured name for the <see cref="EmitterLabel"/></param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.definelabel?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter DefineLabel(out EmitterLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    /// <summary>
    /// Marks the current position with an <see cref="EmitterLabel"/>
    /// </summary>
    /// <param name="label">The label for which to set an index</param>
    /// <exception cref="ArgumentException">If the <paramref name="label"/> has an invalid index</exception>
    /// <exception cref="ArgumentException">If the <paramref name="label"/> has already been marked</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.marklabel?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter MarkLabel(EmitterLabel label);
#endregion
#region EmitCall(i)
    /// <summary>
    /// Puts a <see cref="OpCodes.Call"/>, <see cref="OpCodes.Callvirt"/>, or <see cref="OpCodes.Newobj"/> instruction
    /// onto the stream to call a <see langword="varargs"/> <see cref="MethodInfo"/>
    /// </summary>
    /// <param name="methodInfo">The <see langword="varargs"/> <see cref="MethodInfo"/> to be called</param>
    /// <param name="optionalParameterTypes">The types of the Option arguments if the method is a <see langword="varargs"/> method; otherwise, <see langword="null"/></param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="methodInfo"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcall?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    TEmitter EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes);

    /// <summary>
    /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call
    /// </summary>
    /// <param name="callingConvention">The managed calling conventions to be used</param>
    /// <param name="returnType">The <see cref="Type"/> of the result</param>
    /// <param name="parameterTypes">The types of the required arguments to the instruction</param>
    /// <param name="optionalParameterTypes">The types of the Option arguments for <see langword="varargs"/> calls</param>
    /// <exception cref="ArgumentNullException">If <paramref name="returnType"/> is <see langword="null"/></exception>
    /// <exception cref="InvalidOperationException">If <paramref name="optionalParameterTypes"/> is not <see langword="null"/> or empty but <paramref name="callingConvention"/> does not include the <see cref="CallingConventions.VarArgs"/> flag.</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli?view=net-8.0#system-reflection-emit-ilgenerator-emitcalli(system-reflection-emit-opcode-system-runtime-interopservices-callingconvention-system-type-system-type())">learn.microsoft.com</a>
    /// </links>
    TEmitter EmitCalli(CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes);

    /// <summary>
    /// Puts a <see cref="OpCodes.Calli"/> instruction onto the stream, specifying an unmanaged calling convention for the indirect call
    /// </summary>
    /// <param name="unmanagedCallConv">The unmanaged calling convention to be used</param>
    /// <param name="returnType">The <see cref="Type"/> of the result</param>
    /// <param name="parameterTypes">The types of the required arguments to the instruction</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="returnType"/> is <c>null</c></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.emitcalli?view=net-8.0#system-reflection-emit-ilgenerator-emitcalli(system-reflection-emit-opcode-system-reflection-callingconventions-system-type-system-type()-system-type())">learn.microsoft.com</a>
    /// </links>
    TEmitter EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes);
#endregion
}