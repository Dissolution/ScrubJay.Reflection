// using System.Reflection.Emit;
// using ScrubJay.Reflection.Runtime;
//
// namespace ScrubJay.Reflection.Sandbox;
//
// internal class ILThrowExceptionDemo
// {
//     public static int Original(int a, int b)
//     {
//         int localSum;
//         OverflowException ovx;
//
//         try
//         {
//             if (a > 100)
//                 goto failed;
//             if (b > 100)
//                 goto failed;
//
//             localSum = a + b;
//             goto endOfMethod;
//
//             failed:
//             ovx = new("Cannot accept values over 100 for add.");
//             throw ovx;
//         }
//         catch (OverflowException ex)
//         {
//             ovx = ex;
//             Console.WriteLine("Caught {0}", ovx);
//             localSum = -1;
//         }
//         endOfMethod:
//         return localSum;
//     }
//
//
//     public static void Demo()
//     {
//         Type overflowExceptionType = typeof(OverflowException);
//         ConstructorInfo overflowExceptionCtor = overflowExceptionType
//             .GetConstructor([typeof(string)])
//             .ThrowIfNull();
//         MethodInfo overflowExceptionToStringMethod = overflowExceptionType.GetMethod("ToString")
//             .ThrowIfNull();
//         MethodInfo consoleWriteLineMethod = typeof(Console)
//             .GetMethod("WriteLine", [typeof(string), typeof(object)])
//             .ThrowIfNull();
//
//         var builder = RuntimeBuilder.CreateDynamicILMethod<Func<int, int, int>>();
//         ILGenerator gen = builder.ILGenerator;
//
//
//         LocalBuilder localSum = gen.DeclareLocal(typeof(int));
//         LocalBuilder localOverflowException = gen.DeclareLocal(overflowExceptionType);
//
//         // In order to successfully branch, we need to create labels
//         // representing the offset IL instruction block to branch to.
//         // These labels, when the MarkLabel(Label) method is invoked,
//         // will specify the IL instruction to branch to.
//         //
//         Label lblFailed = gen.DefineLabel();
//         //Label lblEndOfMethod = gen.DefineLabel();
//
//         // Begin the try block.
//         Label lblExBlock = gen.BeginExceptionBlock();
//
//         // First, load argument 0 and the integer value of "100" onto the
//         // stack. If arg0 > 100, branch to the label "failed", which is marked
//         // as the address of the block that throws an exception.
//         //
//         gen.Emit(OpCodes.Ldarg_0);
//         gen.Emit(OpCodes.Ldc_I4, 100);
//         gen.Emit(OpCodes.Bgt, lblFailed);
//
//         // Now, check to see if argument 1 was greater than 100. If it was,
//         // branch to "failed." Otherwise, fall through and perform the addition,
//         // branching unconditionally to the instruction at the label "endOfMthd".
//         //
//         gen.Emit(OpCodes.Ldarg_1);
//         gen.Emit(OpCodes.Ldc_I4, 100);
//         gen.Emit(OpCodes.Bgt, lblFailed);
//
//         gen.Emit(OpCodes.Ldarg_0);
//         gen.Emit(OpCodes.Ldarg_1);
//         gen.Emit(OpCodes.Add);
//         // Store the result of the addition.
//         gen.Emit(OpCodes.Stloc, localSum);
//         //gen.Emit(OpCodes.Br, lblEndOfMethod);
//         gen.Emit(OpCodes.Leave, lblExBlock);
//
//         // If one of the arguments was greater than 100, we need to throw an
//         // exception. We'll use "OverflowException" with a customized message.
//         // First, we load our message onto the stack, and then create a new
//         // exception object using the constructor overload that accepts a
//         // string message.
//         //
//         gen.MarkLabel(lblFailed);
//         gen.Emit(OpCodes.Ldstr, "Cannot accept values over 100 for add.");
//         gen.Emit(OpCodes.Newobj, overflowExceptionCtor);
//
//         // We're going to need to refer to that exception object later, so let's
//         // store it in a temporary variable. Since the store function pops the
//         // the value/reference off the stack, and we'll need it to throw the
//         // exception, we will subsequently load it back onto the stack as well.
//
//         gen.Emit(OpCodes.Stloc, localOverflowException);
//         gen.Emit(OpCodes.Ldloc, localOverflowException);
//
//         // Throw the exception now on the stack.
//
//         gen.Emit(OpCodes.Throw);
//
//         // Start the catch block for OverflowException.
//         //
//         gen.BeginCatchBlock(overflowExceptionType);
//
//         // When we enter the catch block, the thrown exception
//         // is on the stack. Store it, then load the format string
//         // for WriteLine.
//         //
//         gen.Emit(OpCodes.Stloc, localOverflowException);
//         gen.Emit(OpCodes.Ldstr, "Caught {0}");
//
//         // Push the thrown exception back on the stack, then
//         // call its ToString() method. Note that if this catch block
//         // were for a more general exception type, like Exception,
//         // it would be necessary to use the ToString for that type.
//         //
//         gen.Emit(OpCodes.Ldloc, localOverflowException);
//         gen.Emit(OpCodes.Callvirt, overflowExceptionToStringMethod);
//
//         // The format string and the return value from ToString() are
//         // now on the stack. Call WriteLine(string, object).
//         //
//         gen.Emit(OpCodes.Call, consoleWriteLineMethod);
//
//         // Since our function has to return an integer value, we'll load -1 onto
//         // the stack to indicate an error, and store it in local variable tmp1.
//         //
//         gen.Emit(OpCodes.Ldc_I4_M1);
//         gen.Emit(OpCodes.Stloc, localSum);
//
//         //n
//         //gen.Emit(OpCodes.Leave, lblEndOfMethod);
//         gen.Emit(OpCodes.Leave, lblExBlock);
//         
//         // End the exception handling block.
//
//         gen.EndExceptionBlock();
//
//         // The end of the method. If no exception was thrown, the correct value
//         // will be saved in tmp1. If an exception was thrown, tmp1 will be equal
//         // to -1. Either way, we'll load the value of tmp1 onto the stack and return.
//         //
//         //gen.MarkLabel(lblEndOfMethod);
//         gen.Emit(OpCodes.Ldloc, localSum);
//         gen.Emit(OpCodes.Ret);
//
//
//         var del = builder.TryCreateDelegate().OkOrThrow();
//
//         int[] delParams = new int[2];
//
//         Console.Write("Enter an integer value: ");
//         delParams[0] = Convert.ToInt32(Console.ReadLine());
//
//         Console.Write("Enter another integer value: ");
//         delParams[1] = Convert.ToInt32(Console.ReadLine());
//
//         Console.WriteLine("If either integer was > 100, an exception will be thrown.");
//         Console.WriteLine("---");
//
//         var result = del(delParams[0], delParams[1]);
//
//         Console.WriteLine("{0} + {1} = {2}",
//             delParams[0], delParams[1],
//             result);
//
//         
//         //Console.ReadLine();
//     }
// }
//
// /* This code produces output similar to the following:
//
// Enter an integer value: 24
// Enter another integer value: 101
// If either integer was > 100, an exception will be thrown.
// ---
// Caught System.OverflowException: Arithmetic operation resulted in an overflow.
// at Adder.DoAdd(Int32 , Int32 )
// 24 + 101 = -1
// */