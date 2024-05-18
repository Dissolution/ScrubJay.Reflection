// ReSharper disable IdentifierTypo
namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface IOperationEmitter<TEmitter> : IILEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
    /// <summary>
    /// Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfilter?view=netcore-3.0"/>
    TEmitter Endfilter();

    /// <summary>
    /// Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.endfinally"/>
    TEmitter Endfinally();

    /// <summary>
    /// Returns an unmanaged pointer to the argument list of the current method.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.arglist?view=netcore-3.0"/>
    TEmitter Arglist();
    
#region Labels
    /// <summary>
    /// Implements a jump table.
    /// </summary>
    /// <param name="labels">The labels for the jumptable.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is <see langword="null"/> or empty.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.switch"/>
    TEmitter Switch(params EmitterLabel[] labels);
#endregion

#region Method Related (Call)
    /// <summary>
    /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained?view=netcore-3.0"/>
    TEmitter Constrained(Type type);

    /// <summary>
    /// Constrains the <see cref="Type"/> on which a virtual method call (<see cref="OpCodes.Callvirt"/>) is made.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to constrain the <see cref="OpCodes.Callvirt"/> upon.</typeparam>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.constrained?view=netcore-3.0"/>
    TEmitter Constrained<T>();

    /// <summary>
    /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given <see cref="MethodInfo"/> onto the stack.
    /// </summary>
    /// <param name="method">The method to get pointer to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldftn"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
    TEmitter Ldftn(MethodInfo method);

    /// <summary>
    /// Pushes an unmanaged pointer (<see cref="IntPtr"/>) to the native code implementing the given virtual <see cref="MethodInfo"/> onto the stack.
    /// </summary>
    /// <param name="method">The method to get pointer to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldvirtftn"/>
    TEmitter Ldvirtftn(MethodInfo method);

    /// <summary>
    /// Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.tailcall"/>
    TEmitter Tailcall();
#endregion

#region Debugging
    /// <summary>
    /// Signals the Common Language Infrastructure (CLI) to inform the debugger that a breakpoint has been tripped.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.break"/>
    TEmitter Break();

    /// <summary>
    /// Fills space if opcodes are patched. No meaningful operation is performed, although a processing cycle can be consumed.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.nop"/>
    TEmitter Nop();

#region Prefix
    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix1"/>
    [Obsolete("This is a reserved instruction.", true)]
    TEmitter Prefix1();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix2"/>
    [Obsolete("This is a reserved instruction.", true)]
    TEmitter Prefix2();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix3"/>
    [Obsolete("This is a reserved instruction.", true)]
    TEmitter Prefix3();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix4"/>
    [Obsolete("This is a reserved instruction.", true)]
    TEmitter Prefix4();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix5"/>
    [Obsolete("This is a reserved instruction.", true)]
    TEmitter Prefix5();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix6"/>
    [Obsolete("This is a reserved instruction.", true)]
    TEmitter Prefix6();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefix7"/>
    [Obsolete("This is a reserved instruction.", true)]
    TEmitter Prefix7();

    /// <summary>
    /// This is a reserved instruction.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.prefixref"/>
    [Obsolete("This is a reserved instruction.", true)]
    TEmitter Prefixref();
#endregion
#endregion

#region Exceptions
    /// <summary>
    /// Emits the instructions to throw an <see cref="ArithmeticException"/> if the value on the stack is not a finite number.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ckfinite"/>
    TEmitter Ckfinite();

    /// <summary>
    /// Rethrows the current exception.
    /// </summary>
    /// <exception cref="NotSupportedException">The stream being emitted is not currently in an <see langword="catch"/> block.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rethrow"/>
    TEmitter Rethrow();

    /// <summary>
    /// Throws the <see cref="Exception"/> currently on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Exception"/> <see cref="object"/> on the stack is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.throw"/>
    TEmitter Throw();
#endregion

    #region Bitwise
        /// <summary>
    /// Computes the bitwise AND (<see langword="&amp;"/>) of two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.and?view=netcore-3.0"/>
    TEmitter And();

    /// <summary>
    /// Negates a value (<see langword="-"/>) and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.neg"/>
    TEmitter Neg();

    /// <summary>
    /// Computes the one's complement (<see langword="~"/>) of a value and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.not"/>
    TEmitter Not();

    /// <summary>
    /// Computes the bitwise OR (<see langword="|"/>) of two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.or"/>
    TEmitter Or();

    /// <summary>
    /// Shifts an integer value to the left (<see langword="&lt;&lt;"/>) by a specified number of bits, pushing the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shl"/>
    TEmitter Shl();

    /// <summary>
    /// Computes the bitwise XOR (<see langword="^"/>) of a value and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.xor"/>
    TEmitter Xor();
    #endregion
    
#region Branching
    /// <summary>
    /// Exits the current method and jumps to the given <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">The metadata token for a <see cref="MethodInfo"/> to jump to.</param>
    /// <exception cref="ArgumentNullException">If the <paramref name="method"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.jmp"/>
    TEmitter Jmp(MethodInfo method);

    /// <summary>
    /// Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.
    /// </summary>
    TEmitter Ret();
#endregion

#region Boxing / Unboxing / Casting
    /// <summary>
    /// Converts a value into an <see cref="object"/> reference.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of value that is to be boxed.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.box"/>
    TEmitter Box(Type type);

    /// <summary>
    /// Converts a value into an <see cref="object"/> reference.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of value that is to be boxed.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.box"/>
    TEmitter Box<T>();

    /// <summary>
    /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to a value-type pointer.
    /// </summary>
    /// <param name="type">The value type that is to be unboxed.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="type"/> is not a value type.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox"/>
    TEmitter Unbox(Type type);

    /// <summary>
    /// Converts the boxed representation (<see cref="object"/>) of a <see langword="struct"/> to a value-type pointer.
    /// </summary>
    /// <typeparam name="T">The value type that is to be unboxed.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox"/>
    TEmitter Unbox<T>() where T : struct;

    /// <summary>
    /// Converts the boxed representation (<see cref="object"/>) value to its unboxed value.
    /// </summary>
    /// <param name="type">The Type of value to unbox/castclass the value to</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox_any"/>
    TEmitter Unbox_Any(Type type);

    /// <summary>
    /// Converts the boxed representation (<see cref="object"/>) value to its unboxed value.
    /// </summary>
    /// <typeparam name="T">The type that is to be unboxed.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unbox_any"/>
    TEmitter Unbox_Any<T>();

    /// <summary>
    /// Casts an <see cref="object"/> into the given <see langword="class"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of <see langword="class"/> to cast to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="type"/> is not a <see langword="class"/> type.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.castclass"/>
    TEmitter Castclass(Type type);

    /// <summary>
    /// Casts an <see cref="object"/> into the given <see langword="class"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <see langword="class"/> to cast to.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.castclass"/>
    TEmitter Castclass<T>() where T : class;

    /// <summary>
    /// Tests whether an <see cref="object"/> is an instance of a given <see langword="class"/> <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of <see langword="class"/> to cast to.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="type"/> is not a <see langword="class"/> type.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.isinst"/>
    TEmitter Isinst(Type type);

    /// <summary>
    /// Tests whether an <see cref="object"/> is an instance of a given <see langword="class"/> <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <see langword="class"/> to cast to.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.isinst"/>
    TEmitter Isinst<T>();
#endregion


#region Comparison
    /// <summary>
    /// Compares two values. If they are equal (<see langword="=="/>), (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ceq?view=netcore-3.0"/>
    TEmitter Ceq();
#endregion

#region byte*  /  byte[]  /  ref byte
    /// <summary>
    /// Copies a number of bytes from a source address to a destination address.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpblk"/>
    TEmitter Cpblk();

    /// <summary>
    /// Initializes a specified block of memory at a specific address to a given size and initial value.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initblk"/>
    TEmitter Initblk();

    /// <summary>
    /// Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (<see langword="byte*"/>) of the first allocated byte onto the stack.
    /// </summary>
    /// <exception cref="StackOverflowException">If there is insufficient memory to service this request.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.localloc"/>
    TEmitter Localloc();
#endregion

#region Copy / Duplicate
    /// <summary>
    /// Copies the <see langword="struct"/> located at the <see cref="IntPtr"/> source address to the <see cref="IntPtr"/> destination address.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of <see langword="struct"/> that is to be copied.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpobj"/>
    TEmitter Cpobj(Type type);

    /// <summary>
    /// Copies the <see langword="struct"/> located at the <see cref="IntPtr"/> source address to the <see cref="IntPtr"/> destination address.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of <see langword="struct"/> that is to be copied.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpobj"/>
    TEmitter Cpobj<T>()
        where T : struct;

    /// <summary>
    /// Copies a value, and then pushes the copy onto the evaluation stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.dup"/>
    TEmitter Dup();
#endregion

#region Value Transformation / Creation
    /// <summary>
    /// Initializes each field of the <see langword="struct"/> at a specified address to a <see langword="null"/> reference or 0 primitive.
    /// </summary>
    /// <param name="type">The <see langword="struct"/> to be initialized.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="type"/> is not a struct.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initobj"/>
    TEmitter Initobj(Type type);

    /// <summary>
    /// Initializes each field of the <see langword="struct"/> at a specified address to a <see langword="null"/> reference or 0 primitive.
    /// </summary>
    /// <typeparam name="T">The <see langword="struct"/> to be initialized.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.initobj"/>
    TEmitter Initobj<T>()
        where T : struct;

    /// <summary>
    /// Creates a new <see cref="object"/> or <see langword="struct"/> and pushes it onto the stack.
    /// </summary>
    /// <param name="ctor">The <see cref="ConstructorInfo"/> to use to create the object.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="ctor"/> is null.</exception>
    /// <exception cref="OutOfMemoryException">If there is insufficient memory to satisfy the request.</exception>
    /// <exception cref="MissingMethodException">If the <paramref name="ctor"/> could not be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newobj"/>
    TEmitter Newobj(ConstructorInfo ctor);

    /// <summary>
    /// Removes the value currently on top of the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.pop"/>
    TEmitter Pop();
#endregion

#region Load Value
    /// <summary>
    /// Pushes a <see langword="null"/> <see cref="object"/> onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldnull"/>
    TEmitter Ldnull();

    /// <summary>
    /// Pushes a <see cref="string"/> onto the stack.
    /// </summary>
    /// <param name="str">The <see cref="string"/> to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldstr"/>
    TEmitter Ldstr(string str);

#region Ldtoken
    /// <summary>
    /// Converts a metadata token to its runtime representation and pushes it onto the stack.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to convert to a <see cref="RuntimeTypeHandle"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
    TEmitter Ldtoken(Type type);

    /// <summary>
    /// Converts a metadata token to its runtime representation and pushes it onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> to convert to a <see cref="RuntimeFieldHandle"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
    TEmitter Ldtoken(FieldInfo field);

    /// <summary>
    /// Converts a metadata token to its runtime representation and pushes it onto the stack.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> to convert to a <see cref="RuntimeMethodHandle"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldtoken"/>
    TEmitter Ldtoken(MethodInfo method);
#endregion
#endregion

#region Arrays
    /// <summary>
    /// Pushes the number of elements of a zero-based, one-dimensional <see cref="Array"/> onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldlen"/>
    TEmitter Ldlen();

#region Load Element
   

    /// <summary>
    /// Pushes an <see cref="object"/> reference to a new zero-based, one-dimensional <see cref="Array"/> whose elements are the given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <param name="type">The type of values that can be stored in the array.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newarr"/>
    TEmitter Newarr(Type type);

    /// <summary>
    /// Pushes an <see cref="object"/> reference to a new zero-based, one-dimensional <see cref="Array"/> whose elements are the given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of values that can be stored in the array.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.newarr"/>
    TEmitter Newarr<T>();

    /// <summary>
    /// Specifies that the subsequent array address operation performs no type check at run time, and that it returns a managed pointer whose mutability is restricted.
    /// </summary>
    /// <remarks>This instruction can only appear before a <see cref="Ldelema"/> instruction.</remarks>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.readonly"/>
    TEmitter Readonly();
#endregion
    
    /// <summary>
    /// Indicates that an address on the stack might not be aligned to the natural size of the immediately following
    /// <see cref="Ldind"/>, <see cref="Stind"/>, <see cref="Ldfld"/>, <see cref="Stfld"/>, <see cref="Ldobj"/>, <see cref="Stobj"/>, <see cref="Initblk"/>, or <see cref="Cpblk"/> instruction.
    /// </summary>
    /// <param name="alignment">Specifies the generated code should assume the address is <see cref="byte"/>, double-<see cref="byte"/>, or quad-<see cref="byte"/> aligned.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="alignment"/> is not 1, 2, or 4.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.unaligned"/>
    TEmitter Unaligned(int alignment);

    /// <summary>
    /// Indicates that an address currently on the stack might be volatile, and the results of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.volatile"/>
    TEmitter Volatile();
#endregion

#region Upon Type
    /// <summary>
    /// Pushes a typed reference to an instance of a given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of reference to push.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mkrefany"/>
    TEmitter Mkrefany(Type type);

    /// <summary>
    /// Pushes a typed reference to an instance of a given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of reference to push.</typeparam>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mkrefany"/>
    TEmitter Mkrefany<T>();

    /// <summary>
    /// Retrieves the type token embedded in a typed reference.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanytype"/>
    TEmitter Refanytype();

    /// <summary>
    /// Retrieves the address (<see langword="&amp;"/>) embedded in a typed reference.
    /// </summary>
    /// <param name="type">The type of reference to retrieve the address.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
    /// <exception cref="InvalidCastException">If <paramref name="type"/> is not the same as the <see cref="Type"/> of the reference.</exception>
    /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanyval"/>
    TEmitter Refanyval(Type type);

    /// <summary>
    /// Retrieves the address (<see langword="&amp;"/>) embedded in a typed reference.
    /// </summary>
    /// <typeparam name="T">The type of reference to retrieve the address.</typeparam>
    /// <exception cref="InvalidCastException">If <typeparamref name="T"/> is not the same as the <see cref="Type"/> of the reference.</exception>
    /// <exception cref="TypeLoadException">If <typeparamref name="T"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.refanyval"/>
    TEmitter Refanyval<T>();

    /// <summary>
    /// Pushes the size, in <see cref="byte"/>s, of a given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get the size of.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sizeof"/>
    TEmitter Sizeof(Type type);

    /// <summary>
    /// Pushes the size, in <see cref="byte"/>s, of a given <see cref="Type"/> onto the stack.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to get the size of.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown if the given <see cref="Type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sizeof"/>
    TEmitter Sizeof<T>()
        where T : unmanaged;
#endregion
}