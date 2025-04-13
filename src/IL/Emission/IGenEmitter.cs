namespace ScrubJay.Reflection.IL.Emission;

public interface IGenEmitter<S> : IEmitter<S>
    where S : IGenEmitter<S>
{
#region try/catch/finally

    /// <summary>
    /// Begins an exception block for a non-filtered <see cref="Exception"/>
    /// </summary>
    /// <param name="label">
    /// The <see cref="CILLabel"/> that points to the end of the <c>try/catch</c> block<br/>
    /// This will leave you in the correct place to execute <see langword="finally"/> blocks or to finish the <see langword="try"/>.
    /// </param>
    /// <param name="labelName">Automatically captured name for the <see cref="CILLabel"/></param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptionblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S BeginExceptionBlock(out CILLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

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
    S BeginCatchBlock(Type exceptionType);

    /// <summary>
    /// Begins a <c>catch</c> block
    /// </summary>
    /// <typeparam name="TException">The <see cref="Type"/> of <see cref="Exception"/> to catch</typeparam>
    /// <exception cref="ArgumentException">Thrown if within a filtered exception</exception>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.begincatchblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S BeginCatchBlock<TException>()
        where TException : Exception;

    /// <summary>
    /// Begins a <see langword="finally"/> block
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfinallyblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S BeginFinallyBlock();

    /// <summary>
    /// Begins an exception block for a filtered <see cref="Exception"/>
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginexceptfilterblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S BeginExceptFilterBlock();

    /// <summary>
    /// Begins an exception fault block
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginfaultblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S BeginFaultBlock();

    /// <summary>
    /// Ends an exception block
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if this operation occurs in an unexpected place</exception>
    /// <exception cref="NotSupportedException">Thrown if not currently in an exception block</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endexceptionblock?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S EndExceptionBlock();

#endregion

#region Scope

    /// <summary>
    /// Begins a lexical scope
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginscope?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S BeginScope();

    /// <summary>
    /// Ends a lexical scope
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endscope?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S EndScope();

    /// <summary>
    /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope
    /// </summary>
    /// <param name="namespace">The namespace to be used in evaluating locals and watches for the current active lexical scope</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="namespace"/> is <c>null</c> or has a <see cref="string.Length"/> of 0</exception>
    /// <exception cref="NotSupportedException">Thrown if emitting to a <see cref="DynamicMethod"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.usingnamespace?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S UsingNamespace(string @namespace);

#endregion

#region Locals

    /// <summary>
    /// Declares a <see cref="CILLocal"/> variable
    /// </summary>
    /// <param name="localType">The type of the <see cref="CILLocal"/></param>
    /// <param name="local">Outputs the declared <see cref="CILLocal"/></param>
    /// <param name="localName">Automatically captured name for the <see cref="CILLocal"/></param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="localType"/> is <c>null</c></exception>
    /// <exception cref="InvalidOperationException">Thrown if within <see cref="TypeBuilder.CreateType"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=net-8.0#system-reflection-emit-ilgenerator-declarelocal(system-type)">learn.microsoft.com</a>
    /// </links>
    S DeclareLocal(Type localType, out CILLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);

    /// <summary>
    /// Declares a <see cref="CILLocal"/> variable
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the <see cref="CILLocal"/></typeparam>
    /// <param name="local">Outputs the declared <see cref="CILLocal"/></param>
    /// <param name="localName">Automatically captured name for the <see cref="CILLocal"/></param>
    /// <exception cref="InvalidOperationException">Thrown if within <see cref="TypeBuilder.CreateType"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=net-8.0#system-reflection-emit-ilgenerator-declarelocal(system-type)">learn.microsoft.com</a>
    /// </links>
    S DeclareLocal<T>(out CILLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);

    /// <summary>
    /// Declares a <see cref="CILLocal"/> variable
    /// </summary>
    /// <param name="localType">The type of the <see cref="CILLocal"/></param>
    /// <param name="pinned">Whether or not the <see cref="CILLocal"/> should be pinned in memory</param>
    /// <param name="local">Outputs the declared <see cref="CILLocal"/></param>
    /// <param name="localName">Automatically captured name for the <see cref="CILLocal"/></param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="localType"/> is <c>null</c></exception>
    /// <exception cref="InvalidOperationException">Thrown if within <see cref="TypeBuilder.CreateType"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=net-8.0#system-reflection-emit-ilgenerator-declarelocal(system-type-system-boolean)">learn.microsoft.com</a>
    /// </links>
    S DeclareLocal(Type localType, bool pinned, out CILLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);

    /// <summary>
    /// Declares a <see cref="CILLocal"/> variable
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the <see cref="CILLocal"/></typeparam>
    /// <param name="pinned">Whether or not the <see cref="CILLocal"/> should be pinned in memory</param>
    /// <param name="local">Outputs the declared <see cref="CILLocal"/></param>
    /// <param name="localName">Automatically captured name for the <see cref="CILLocal"/></param>
    /// <exception cref="InvalidOperationException">Thrown if within <see cref="TypeBuilder.CreateType"/></exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.declarelocal?view=net-8.0#system-reflection-emit-ilgenerator-declarelocal(system-type-system-boolean)">learn.microsoft.com</a>
    /// </links>
    S DeclareLocal<T>(bool pinned, out CILLocal local, [CallerArgumentExpression(nameof(local))] string? localName = null);

#endregion

#region Labels

    /// <summary>
    /// Declares a new <see cref="CILLabel"/>
    /// </summary>
    /// <param name="label">Outputs the declared <see cref="CILLabel"/></param>
    /// <param name="labelName">Automatically captured name for the <see cref="CILLabel"/></param>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.definelabel?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S DefineLabel(out CILLabel label, [CallerArgumentExpression(nameof(label))] string? labelName = null);

    /// <summary>
    /// Marks the current position with an <see cref="CILLabel"/>
    /// </summary>
    /// <param name="label">The label for which to set an index</param>
    /// <exception cref="ArgumentException">If the <paramref name="label"/> has an invalid index</exception>
    /// <exception cref="ArgumentException">If the <paramref name="label"/> has already been marked</exception>
    /// <links>
    /// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.marklabel?view=net-8.0">learn.microsoft.com</a>
    /// </links>
    S MarkLabel(CILLabel label);

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
    S EmitCall(MethodInfo methodInfo, Type[]? optionalParameterTypes);

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
    S EmitCalli(CallingConventions callingConvention, Type? returnType, Type[]? parameterTypes, Type[]? optionalParameterTypes);

#if !NETSTANDARD2_0
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
    S EmitCalli(CallingConvention unmanagedCallConv, Type? returnType, Type[]? parameterTypes);
#endif

#endregion
}