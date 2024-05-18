// ReSharper disable IdentifierTypo
namespace ScrubJay.Reflection.Runtime.Emission.Emitters;

public interface IDirectOperationEmitter<TEmitter> : 
    IOperationEmitter<TEmitter>,
    IILEmitter<TEmitter>
    where TEmitter : IILEmitter<TEmitter>
{
#region Arguments


#region Ldarg
    /// <summary>
    /// Loads the argument with the specified <paramref name="index"/> onto the stack.
    /// </summary>
    /// <param name="index">The index of the argument to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg"/>
    TEmitter Ldarg(short index);

    /// <summary>
    /// Loads the argument with the specified short-form <paramref name="index"/> onto the stack.
    /// </summary>
    /// <param name="index">The short-form index of the argument to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_s"/>
    TEmitter Ldarg_S(byte index);

    /// <summary>
    /// Loads the argument at index 0 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_0"/>
    TEmitter Ldarg_0();

    /// <summary>
    /// Loads the argument at index 1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_1"/>
    TEmitter Ldarg_1();

    /// <summary>
    /// Loads the argument at index 2 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_2"/>
    TEmitter Ldarg_2();

    /// <summary>
    /// Loads the argument at index 3 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarg_3"/>
    TEmitter Ldarg_3();

    /// <summary>
    /// Loads the address of the argument with the specified <paramref name="index"/> onto the stack.
    /// </summary>
    /// <param name="index">The index of the argument address to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga"/>
    TEmitter Ldarga(short index);

    /// <summary>
    /// Loads the address of the argument with the specified short-form <paramref name="index"/> onto the stack.
    /// </summary>
    /// <param name="index">The short-form index of the argument address to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is invalid.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldarga_s"/>
    TEmitter Ldarga_S(byte index);
#endregion
#region Starg
    /// <summary>
    /// Stores the value on top of the stack in the argument at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index of the argument.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg"/>
    TEmitter Starg(short index);

    /// <summary>
    /// Stores the value on top of the stack in the argument at the given short-form <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The short-form index of the argument.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.starg_s"/>
    TEmitter Starg_S(byte index);
#endregion
#endregion

#region Locals
#region Load
    /// <summary>
    /// Loads the given <see cref="EmitterLocal"/>'s value onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
    TEmitter Ldloc(short index);
    
    /// <summary>
    /// Loads the given <see cref="EmitterLocal"/>'s value onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
    TEmitter Ldloc(EmitterLocal local);

    /// <summary>
    /// Loads the given short-form <see cref="EmitterLocal"/>'s value onto the stack.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
    TEmitter Ldloc_S(byte index);
    
    /// <summary>
    /// Loads the given short-form <see cref="EmitterLocal"/>'s value onto the stack.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_s"/>
    TEmitter Ldloc_S(EmitterLocal local);

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 0 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_0"/>
    TEmitter Ldloc_0();

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_1"/>
    TEmitter Ldloc_1();

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 2 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_2"/>
    TEmitter Ldloc_2();

    /// <summary>
    /// Loads the value of the <see cref="EmitterLocal"/> variable at index 3 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloc_3"/>
    TEmitter Ldloc_3();

    /// <summary>
    /// Loads the address of the given <see cref="EmitterLocal"/> variable.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
    TEmitter Ldloca(EmitterLocal local);

    /// <summary>
    /// Loads the address of the given short-form <see cref="EmitterLocal"/> variable.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="local"/> is not short-form.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldloca_s"/>
    TEmitter Ldloca_S(EmitterLocal local);
#endregion

#region Store
    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the given <see cref="EmitterLocal"/>.
    /// </summary>
    /// <param name="index">The index of the Local Variable to store the value in.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
    TEmitter Stloc(short index);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the given short-form <see cref="EmitterLocal"/>.
    /// </summary>
    /// <param name="index">The index of the Local Variable to store the value in.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
    TEmitter Stloc_S(byte index);
    
    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the given <see cref="EmitterLocal"/>.
    /// </summary>
    /// <param name="local">The <see cref="EmitterLocal"/> to store the value in.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
    TEmitter Stloc(EmitterLocal local);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the given short-form <see cref="EmitterLocal"/>.
    /// </summary>
    /// <param name="local">The short-form <see cref="EmitterLocal"/> to store the value in.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_s"/>
    TEmitter Stloc_S(EmitterLocal local);

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 0.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_0"/>
    TEmitter Stloc_0();

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 1.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_1"/>
    TEmitter Stloc_1();

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 2.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_2"/>
    TEmitter Stloc_2();

    /// <summary>
    /// Pops the value from the top of the stack and stores it in a the <see cref="EmitterLocal"/> at index 3.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stloc_3"/>
    TEmitter Stloc_3();
#endregion
#endregion
    
#region Method Related (Call)
    /// <summary>
    /// Calls the given <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is null.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.call"/>
    TEmitter Call(MethodInfo method);

    /// <summary>
    /// Calls the given late-bound <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo"/> that will be called.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.callvirt"/>
    TEmitter Callvirt(MethodInfo method);
#endregion

#region Math
    /// <summary>
    /// Adds two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add?view=netcore-3.0"/>
    TEmitter Add();

    /// <summary>
    /// Adds two <see cref="int"/>s, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add_ovf?view=netcore-3.0"/>
    TEmitter Add_Ovf();

    /// <summary>
    /// Adds two <see cref="uint"/>s, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.add_ovf_un?view=netcore-3.0"/>
    TEmitter Add_Ovf_Un();

    /// <summary>
    /// Divides two values and pushes the result as a <see cref="float"/> or <see cref="int"/> quotient onto the evaluation stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.div"/>
    TEmitter Div();

    /// <summary>
    /// Divides two unsigned values and pushes the result as a <see cref="int"/> quotient onto the evaluation stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.div_un"/>
    TEmitter Div_Un();

    /// <summary>
    /// Multiplies two values and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul"/>
    TEmitter Mul();

    /// <summary>
    /// Multiplies two integer values, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul_ovf"/>
    TEmitter Mul_Ovf();

    /// <summary>
    /// Multiplies two unsigned integer values, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.mul_ovf_un"/>
    TEmitter Mul_Ovf_Un();

    /// <summary>
    /// Divides two values and pushes the remainder onto the evaluation stack.
    /// </summary>
    /// <exception cref="DivideByZeroException">If the second value is zero.</exception>
    /// <exception cref="OverflowException">If computing the remainder between <see cref="int.MinValue"/> and <see langword="-1"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rem"/>
    TEmitter Rem();

    /// <summary>
    /// Divides two unsigned values and pushes the remainder onto the evaluation stack.
    /// </summary>
    /// <exception cref="DivideByZeroException">If the second value is zero.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.rem_un"/>
    TEmitter Rem_Un();

    /// <summary>
    /// Subtracts one value from another and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub"/>
    TEmitter Sub();

    /// <summary>
    /// Subtracts one integer value from another, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub_ovf"/>
    TEmitter Sub_Ovf();

    /// <summary>
    /// Subtracts one unsigned integer value from another, performs an <see langword="overflow"/> check, and pushes the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.sub_ovf_un"/>
    TEmitter Sub_Ovf_Un();
#endregion

#region Bitwise
    /// <summary>
    /// Shifts an integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr"/>
    TEmitter Shr();

    /// <summary>
    /// Shifts an unsigned integer value to the right (<see langword="&gt;&gt;"/>) by a specified number of bits, pushing the result onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shr_un"/>
    TEmitter Shr_Un();
#endregion

#region Branching
#region Unconditional
    /// <summary>
    /// Unconditionally transfers control to the given <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br_s?view=netcore-3.0"/>
    TEmitter Br(EmitterLabel label);

    /// <summary>
    /// Unconditionally transfers control to the given short-form <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.br_s?view=netcore-3.0"/>
    TEmitter Br_S(EmitterLabel label);

    /// <summary>
    /// Exits a internal region of code, unconditionally transferring control to the given <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave_s"/>
    TEmitter Leave(EmitterLabel label);

    /// <summary>
    /// Exits a internal region of code, unconditionally transferring control to the given short-form <see cref="Label"/>.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> is not short-form.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.leave_s"/>
    TEmitter Leave_S(EmitterLabel label);
#endregion

#region True
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if value is <see langword="true"/>, not-<see langword="null"/>, or non-zero.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue_s?view=netcore-3.0"/>
    TEmitter Brtrue(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if value is <see langword="true"/>, not-<see langword="null"/>, or non-zero.
    /// </summary>
    /// <param name="label">The short-form<see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brtrue_s?view=netcore-3.0"/>
    TEmitter Brtrue_S(EmitterLabel label);
#endregion
#region False
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if value is <see langword="false"/>, <see langword="null"/>, or zero.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse_s?view=netcore-3.0"/>
    TEmitter Brfalse(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if value is <see langword="false"/>, <see langword="null"/>, or zero.
    /// </summary>
    /// <param name="label">The short-form<see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.brfalse_s?view=netcore-3.0"/>
    TEmitter Brfalse_S(EmitterLabel label);
#endregion
#region ==
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if two values are equal.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq_s?view=netcore-3.0"/>
    TEmitter Beq(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if two values are equal.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.beq_s?view=netcore-3.0"/>
    TEmitter Beq_S(EmitterLabel label);
#endregion
#region !=
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if two unsigned or unordered values are not equal (<see langword="!="/>).
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un_s?view=netcore-3.0"/>
    TEmitter Bne_Un(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if two unsigned or unordered values are not equal (<see langword="!="/>).
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bne_un_s?view=netcore-3.0"/>
    TEmitter Bne_Un_S(EmitterLabel label);
#endregion
#region >=
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_s?view=netcore-3.0"/>
    TEmitter Bge(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_s?view=netcore-3.0"/>
    TEmitter Bge_S(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un_s?view=netcore-3.0"/>
    TEmitter Bge_Un(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than or equal to (<see langword="&gt;="/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bge_un_s?view=netcore-3.0"/>
    TEmitter Bge_Un_S(EmitterLabel label);
#endregion
#region >
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_s?view=netcore-3.0"/>
    TEmitter Bgt(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_s?view=netcore-3.0"/>
    TEmitter Bgt_S(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un_s?view=netcore-3.0"/>
    TEmitter Bgt_Un(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is greater than (<see langword="&gt;"/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.bgt_un_s?view=netcore-3.0"/>
    TEmitter Bgt_Un_S(EmitterLabel label);
#endregion
#region <=
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_s?view=netcore-3.0"/>
    TEmitter Ble(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_s?view=netcore-3.0"/>
    TEmitter Ble_S(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un_s?view=netcore-3.0"/>
    TEmitter Ble_Un(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than or equal to (<see langword="&lt;="/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ble_un_s?view=netcore-3.0"/>
    TEmitter Ble_Un_S(EmitterLabel label);
#endregion
#region <
    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_s?view=netcore-3.0"/>
    TEmitter Blt(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_s?view=netcore-3.0"/>
    TEmitter Blt_S(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The <see cref="Label"/> to transfer to.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un?view=netcore-3.0"/>
    /// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un_s?view=netcore-3.0"/>
    TEmitter Blt_Un(EmitterLabel label);

    /// <summary>
    /// Transfers control to the given short-form <see cref="Label"/> if the first value is less than (<see langword="&lt;"/>) the second value when comparing unsigned integer values or unordered float values.
    /// </summary>
    /// <param name="label">The short-form <see cref="Label"/> to transfer to.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="label"/> does not qualify for short-form instructions.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.blt_un_s?view=netcore-3.0"/>
    TEmitter Blt_Un_S(EmitterLabel label);
#endregion
#endregion
    
#region Conv
#region nativeint
    /// <summary>
    /// Converts the value on the stack to a <see cref="IntPtr"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i?view=netcore-3.0"/>
    TEmitter Conv_I();

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I();

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I_Un();
#endregion
#region sbyte
    /// <summary>
    /// Converts the value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i1?view=netcore-3.0"/>
    TEmitter Conv_I1();

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i1?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I1();

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="sbyte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i1_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I1_Un();
#endregion
#region short
    /// <summary>
    /// Converts the value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i2?view=netcore-3.0"/>
    TEmitter Conv_I2();

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i2?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I2();

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="short"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i2_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I2_Un();
#endregion
#region int
    /// <summary>
    /// Converts the value on the stack to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i4?view=netcore-3.0"/>
    TEmitter Conv_I4();

    /// <summary>
    /// Converts the signed value on the stack to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i4?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I4();

    /// <summary>
    /// Converts the unsigned value on the stack to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i4_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I4_Un();
#endregion
#region long
    /// <summary>
    /// Converts the value on the stack to a <see cref="long"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_i8?view=netcore-3.0"/>
    TEmitter Conv_I8();

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i8?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I8();

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_i8_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_I8_Un();
#endregion

#region nativeuuint
    /// <summary>
    /// Converts the value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u?view=netcore-3.0"/>
    TEmitter Conv_U();

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U();

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="UIntPtr"/>, then extends it to <see cref="IntPtr"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U_Un();
#endregion
#region byte
    /// <summary>
    /// Converts the value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u1?view=netcore-3.0"/>
    TEmitter Conv_U1();

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u1?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U1();

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="byte"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u1_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U1_Un();
#endregion
#region uushort
    /// <summary>
    /// Converts the value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u2?view=netcore-3.0"/>
    TEmitter Conv_U2();

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u2?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U2();

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="ushort"/>, then pads/extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u2_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U2_Un();
#endregion
#region uuint
    /// <summary>
    /// Converts the value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u4?view=netcore-3.0"/>
    TEmitter Conv_U4();

    /// <summary>
    /// Converts the signed value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u4?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U4();

    /// <summary>
    /// Converts the unsigned value on the stack to an <see cref="uint"/>, then extends it to an <see cref="int"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u4_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U4_Un();
#endregion
#region uulong
    /// <summary>
    /// Converts the value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_u8?view=netcore-3.0"/>
    TEmitter Conv_U8();

    /// <summary>
    /// Converts the signed value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u8?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U8();

    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="ulong"/>, then extends it to an <see cref="long"/>, throwing an <see cref="OverflowException"/> on overflow.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_ovf_u8_un?view=netcore-3.0"/>
    TEmitter Conv_Ovf_U8_Un();
#endregion
#region float / double
    /// <summary>
    /// Converts the unsigned value on the stack to a <see cref="float"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r_un?view=netcore-3.0"/>
    TEmitter Conv_R_Un();

    /// <summary>
    /// Converts the value on the stack to a <see cref="float"/>.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r4?view=netcore-3.0"/>
    TEmitter Conv_R4();

    /// <summary>
    /// Converts the value on the stack to a <see cref="double"/>.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.conv_r8"/>
    TEmitter Conv_R8();
#endregion
#endregion

#region Comparison
    /// <summary>
    /// Compares two values. If the first value is greater than (<see langword="&gt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cgt?view=netcore-3.0"/>
    TEmitter Cgt();

    /// <summary>
    /// Compares two unsigned or unordered values. If the first value is greater than (<see langword="&gt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cgt_un?view=netcore-3.0"/>
    TEmitter Cgt_Un();

    /// <summary>
    /// Compares two values. If the first value is less than (<see langword="&lt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.clt?view=netcore-3.0"/>
    TEmitter Clt();

    /// <summary>
    /// Compares two unsigned or unordered values. If the first value is less than (<see langword="&lt;"/>) the second, (<see cref="int"/>)1 is pushed onto the evaluation stack; otherwise (<see cref="int"/>)0 is pushed onto the evaluation stack.
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.clt_un?view=netcore-3.0"/>
    TEmitter Clt_Un();
#endregion

#region Load Value
#region LoaD Constant (LDC)
    /// <summary>
    /// Pushes the given <see cref="int"/> onto the stack.
    /// </summary>
    /// <param name="value">The value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4"/>
    TEmitter Ldc_I4(int value);

    /// <summary>
    /// Pushes the given <see cref="sbyte"/> onto the stack.
    /// </summary>
    /// <param name="value">The short-form value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_s"/>
    TEmitter Ldc_I4_S(sbyte value);

    /// <summary>
    /// Pushes the given <see cref="int"/> value of -1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_m1"/>
    TEmitter Ldc_I4_M1();

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 0 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_0"/>
    TEmitter Ldc_I4_0();

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 1 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_1"/>
    TEmitter Ldc_I4_1();

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 2 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_2"/>
    TEmitter Ldc_I4_2();

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 3 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_3"/>
    TEmitter Ldc_I4_3();

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 4 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_4"/>
    TEmitter Ldc_I4_4();

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 5 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_5"/>
    TEmitter Ldc_I4_5();

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 6 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_6"/>
    TEmitter Ldc_I4_6();

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 7 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_7"/>
    TEmitter Ldc_I4_7();

    /// <summary>
    /// Pushes the given <see cref="int"/> value of 8 onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_8"/>
    TEmitter Ldc_I4_8();

    /// <summary>
    /// Pushes the given <see cref="long"/> onto the stack.
    /// </summary>
    /// <param name="value">The value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i8"/>
    TEmitter Ldc_I8(long value);

    /// <summary>
    /// Pushes the given <see cref="float"/> onto the stack.
    /// </summary>
    /// <param name="value">The value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_r4"/>
    TEmitter Ldc_R4(float value);

    /// <summary>
    /// Pushes the given <see cref="double"/> onto the stack.
    /// </summary>
    /// <param name="value">The value to push onto the stack.</param>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_r8"/>
    TEmitter Ldc_R8(double value);
#endregion
    
#endregion

#region Arrays
   

#region Load Element
    /// <summary>
    /// Loads the element from an array index onto the stack as the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the element to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem"/>
    TEmitter Ldelem(Type type);

    /// <summary>
    /// Loads the element from an array index onto the stack as the given <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the element to load.</typeparam>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem"/>
    TEmitter Ldelem<T>();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="IntPtr"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="IntPtr"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i"/>
    TEmitter Ldelem_I();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="sbyte"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="sbyte"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i1"/>
    TEmitter Ldelem_I1();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="short"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="short"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i2"/>
    TEmitter Ldelem_I2();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="int"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i4"/>
    TEmitter Ldelem_I4();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="long"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="long"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_i8"/>
    TEmitter Ldelem_I8();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="byte"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="byte"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u1"/>
    TEmitter Ldelem_U1();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="ushort"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="ushort"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u2"/>
    TEmitter Ldelem_U2();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="uint"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="uint"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_u4"/>
    TEmitter Ldelem_U4();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="float"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="float"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_r4"/>
    TEmitter Ldelem_R4();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="double"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="double"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_r8"/>
    TEmitter Ldelem_R8();

    /// <summary>
    /// Loads the element from an array index onto the stack as a <see cref="object"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="object"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelem_ref"/>
    TEmitter Ldelem_Ref();

    /// <summary>
    /// Loads the element from an array index onto the stack as an address to a value of the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the element to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelema"/>
    TEmitter Ldelema(Type type);

    /// <summary>
    /// Loads the element from an array index onto the stack as an address to a value of the given <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the element to load.</typeparam>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldelema"/>
    TEmitter Ldelema<T>();
#endregion
#region Store Element
    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the value on the stack with the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the element to store.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is null.</exception>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <paramref name="type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem"/>
    TEmitter Stelem(Type type);

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the value on the stack with the given <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of the element to store.</typeparam>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold elements of the given <see cref="Type"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem"/>
    TEmitter Stelem<T>();

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="IntPtr"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="IntPtr"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i"/>
    TEmitter Stelem_I();

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="sbyte"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="sbyte"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i1"/>
    TEmitter Stelem_I1();

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="short"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="short"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i2"/>
    TEmitter Stelem_I2();

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="int"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="int"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i4"/>
    TEmitter Stelem_I4();

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="long"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="long"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_i8"/>
    TEmitter Stelem_I8();

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="float"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="float"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_r4"/>
    TEmitter Stelem_R4();

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="double"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="double"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_r8"/>
    TEmitter Stelem_R8();

    /// <summary>
    /// Replaces the <see cref="Array"/> element at a given index with the <see cref="object"/> value on the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If the <see cref="Array"/> on the stack is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">If the index on the stack is negative or larger than the upper bound of the <see cref="Array"/>.</exception>
    /// <exception cref="ArrayTypeMismatchException">If the <see cref="Array"/> does not hold <see cref="object"/> elements.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stelem_ref"/>
    TEmitter Stelem_Ref();
#endregion
#endregion

#region Fields
    /// <summary>
    /// Loads the value of the given <see cref="FieldInfo"/> onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose value to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldfld"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsfld"/>
    TEmitter Ldfld(FieldInfo field);

    /// <summary>
    /// Loads the value of the given static <see cref="FieldInfo"/> onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose value to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsfld"/>
    TEmitter Ldsfld(FieldInfo field);

    /// <summary>
    /// Loads the address of the given <see cref="FieldInfo"/> onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose address to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldflda"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsflda"/>
    TEmitter Ldflda(FieldInfo field);

    /// <summary>
    /// Loads the address of the given <see cref="FieldInfo"/> onto the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose address to load.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldsflda"/>
    TEmitter Ldsflda(FieldInfo field);

    /// <summary>
    /// Replaces the value stored in the given <see cref="FieldInfo"/> with the value on the stack.
    /// </summary>
    /// <param name="field">The <see cref="FieldInfo"/> whose value to replace.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="NullReferenceException">If the instance value/pointer on the stack is <see langword="null"/> and the <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stfld"/>
    /// <seealso href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stsfld"/>
    TEmitter Stfld(FieldInfo field);

    /// <summary>
    /// Replaces the value stored in the given static <see cref="FieldInfo"/> with the value on the stack.
    /// </summary>
    /// <param name="field">The static <see cref="FieldInfo"/> whose value to replace.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="field"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="NullReferenceException">If the instance value/pointer on the stack is <see langword="null"/> and the <paramref name="field"/> is not <see langword="static"/>.</exception>
    /// <exception cref="MissingFieldException">If <paramref name="field"/> is not found in metadata.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stsfld"/>
    TEmitter Stsfld(FieldInfo field);
#endregion

#region Load / Store via Address
    /// <summary>
    /// Loads a value from an address onto the stack.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
    TEmitter Ldobj(Type type);

    /// <summary>
    /// Loads a value from an address onto the stack.
    /// </summary>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldobj"/>
    TEmitter Ldobj<T>();

#region Ldind
    /// <summary>
    /// Loads a <see cref="IntPtr"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i"/>
    TEmitter Ldind_I();

    /// <summary>
    /// Loads a <see cref="sbyte"/> value from an address onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i1"/>
    TEmitter Ldind_I1();

    /// <summary>
    /// Loads a <see cref="short"/> value from an address onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i2"/>
    TEmitter Ldind_I2();

    /// <summary>
    /// Loads a <see cref="int"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i4"/>
    TEmitter Ldind_I4();

    /// <summary>
    /// Loads a <see cref="long"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_i8"/>
    TEmitter Ldind_I8();

    /// <summary>
    /// Loads a <see cref="byte"/> value from an address onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u1"/>
    TEmitter Ldind_U1();

    /// <summary>
    /// Loads a <see cref="ushort"/> value from an address onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u2"/>
    TEmitter Ldind_U2();

    /// <summary>
    /// Loads a <see cref="uint"/> value from an address onto the stack onto the stack as an <see cref="int"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_u4"/>
    TEmitter Ldind_U4();

    /// <summary>
    /// Loads a <see cref="float"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_r4"/>
    TEmitter Ldind_R4();

    /// <summary>
    /// Loads a <see cref="double"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_r8"/>
    TEmitter Ldind_R8();

    /// <summary>
    /// Loads a <see cref="object"/> value from an address onto the stack.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldind_ref"/>
    TEmitter Ldind_Ref();
#endregion

    /// <summary>
    /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="TypeLoadException">If <paramref name="type"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
    TEmitter Stobj(Type type);

    /// <summary>
    /// Copies a value of the given <see cref="Type"/> from the stack into a supplied memory address.
    /// </summary>
    /// <exception cref="TypeLoadException">If the given <see cref="Type"/> cannot be found.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stobj"/>
    TEmitter Stobj<T>();

#region Stind
    /// <summary>
    /// Stores a <see cref="IntPtr"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i"/>
    TEmitter Stind_I();

    /// <summary>
    /// Stores a <see cref="sbyte"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i1"/>
    TEmitter Stind_I1();

    /// <summary>
    /// Stores a <see cref="short"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i2"/>
    TEmitter Stind_I2();

    /// <summary>
    /// Stores a <see cref="int"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i4"/>
    TEmitter Stind_I4();

    /// <summary>
    /// Stores a <see cref="long"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_i8"/>
    TEmitter Stind_I8();

    /// <summary>
    /// Stores a <see cref="float"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_r4"/>
    TEmitter Stind_R4();

    /// <summary>
    /// Stores a <see cref="double"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_r8"/>
    TEmitter Stind_R8();

    /// <summary>
    /// Stores a <see cref="object"/> value in a supplied address.
    /// </summary>
    /// <exception cref="NullReferenceException">If an invalid address is detected.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.stind_ref"/>
    TEmitter Stind_Ref();
#endregion
#endregion
}