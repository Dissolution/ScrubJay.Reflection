namespace ScrubJay.Sigil.Emission.NonGeneric;

/// <summary>
/// A version of Emit with shorter named versions of it's methods.
///
/// Method names map more or less to OpCodes fields.
/// </summary>
public class EmitShorthand
{
    private readonly Emit _innerEmit;

    /// <summary>
    /// Returns true if this Emit can make use of unverifiable instructions.
    /// </summary>
    public bool AllowsUnverifiableCIL { get { return _innerEmit.AllowsUnverifiableCIL; } }

    /// <summary>
    /// Returns the maxmimum number of items on the stack for the IL stream created with the current emit.
    ///
    /// This is not the maximum that *can be placed*, but the maximum that actually are.
    /// </summary>
    public int MaxStackSize { get { return _innerEmit.MaxStackSize; } }

    /// <summary>
    /// Lookup for the locals currently in scope by name.
    ///
    /// Locals go out of scope when released (by calling Dispose() directly, or via using) and go into scope
    /// immediately after a DeclareLocal()
    /// </summary>
    public LocalLookup Locals { get { return _innerEmit.Locals; } }

    /// <summary>
    /// Lookup for declared labels by name.
    /// </summary>
    public LabelLookup Labels { get { return _innerEmit.Labels; } }

    internal EmitShorthand(Emit inner)
    {
        _innerEmit = inner;
    }

    /// <summary>
    /// Returns the original Emit instance that AsShorthand() was called on.
    /// </summary>
    public Emit AsLonghand()
    {
        return _innerEmit;
    }

    /// <summary>
    /// Returns a string representation of the CIL opcodes written to this Emit to date.
    ///
    /// This method is meant for debugging purposes only.
    /// </summary>
    public string Instructions()
    {
        return _innerEmit.Instructions();
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DeclareLocal``1(System.String)" />
    public SigilLocal DeclareLocal<T>(string name = null)
    {
        return DeclareLocal(typeof(T), name);
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DeclareLocal``1(System.String)" />
    public EmitShorthand DeclareLocal<T>(out SigilLocal sigilLocal, string name = null)
    {
        sigilLocal = DeclareLocal<T>(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DeclareLocal(System.Type, System.String)" />
    public SigilLocal DeclareLocal(Type type, string name = null)
    {
        return _innerEmit.DeclareLocal(type, name);
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DeclareLocal(System.Type, System.String)" />
    public EmitShorthand DeclareLocal(Type type, out SigilLocal sigilLocal, string name = null)
    {
        sigilLocal = DeclareLocal(type, name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DefineLabel(System.String)" />
    public SigilLabel DefineLabel(string name = null)
    {
        return _innerEmit.DefineLabel(name);
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.DefineLabel(System.String)" />
    public EmitShorthand DefineLabel(out SigilLabel sigilLabel, string name = null)
    {
        sigilLabel = DefineLabel(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.MarkLabel(Sigil.Label, IEnumerable``1)" />
    public EmitShorthand MarkLabel(SigilLabel sigilLabel)
    {
        _innerEmit.MarkLabel(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.MarkLabel(System.String, IEnumerable``1)" />
    public EmitShorthand MarkLabel(string name)
    {
        _innerEmit.MarkLabel(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginExceptionBlock" />
    public ExceptionBlock BeginExceptionBlock()
    {
        return _innerEmit.BeginExceptionBlock();
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginExceptionBlock" />
    public EmitShorthand BeginExceptionBlock(out ExceptionBlock forTry)
    {
        forTry = BeginExceptionBlock();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginCatchBlock``1(Sigil.ExceptionBlock)" />
    public CatchBlock BeginCatchBlock<TExceptionType>(ExceptionBlock forTry)
    {
        return BeginCatchBlock(forTry, typeof(TExceptionType));
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginCatchBlock``1(Sigil.ExceptionBlock)" />
    public EmitShorthand BeginCatchBlock<TExceptionType>(ExceptionBlock forTry, out CatchBlock tryCatch)
    {
        tryCatch = BeginCatchBlock<TExceptionType>(forTry);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginCatchBlock(System.Type, Sigil.ExceptionBlock)" />
    public CatchBlock BeginCatchBlock(ExceptionBlock forTry, Type exceptionType)
    {
        return _innerEmit.BeginCatchBlock(forTry, exceptionType);
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginCatchBlock(System.Type, Sigil.ExceptionBlock)" />
    public EmitShorthand BeginCatchBlock(ExceptionBlock forTry, Type exceptionType, out CatchBlock forCatch)
    {
        forCatch = BeginCatchBlock(forTry, exceptionType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.EndCatchBlock(Sigil.CatchBlock)" />
    public EmitShorthand EndCatchBlock(CatchBlock forCatch)
    {
        _innerEmit.EndCatchBlock(forCatch);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginFinallyBlock(Sigil.ExceptionBlock)" />
    public FinallyBlock BeginFinallyBlock(ExceptionBlock forTry)
    {
        return _innerEmit.BeginFinallyBlock(forTry);
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BeginFinallyBlock(Sigil.ExceptionBlock)" />
    public EmitShorthand BeginFinallyBlock(ExceptionBlock forTry, out FinallyBlock forFinally)
    {
        forFinally = BeginFinallyBlock(forTry);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.EndFinallyBlock(Sigil.FinallyBlock)" />
    public EmitShorthand EndFinallyBlock(FinallyBlock forFinally)
    {
        _innerEmit.EndFinallyBlock(forFinally);
        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.EndExceptionBlock(Sigil.ExceptionBlock)" />
    public EmitShorthand EndExceptionBlock(ExceptionBlock forTry)
    {
        _innerEmit.EndExceptionBlock(forTry);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CreateDelegate(System.Type, Sigil.OptimizationOptions)" />
    public object CreateDelegate(Type delegateType, OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        return _innerEmit.CreateDelegate(delegateType, optimizationOptions);
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CreateDelegate`2" />
    public TDelegateType CreateDelegate<TDelegateType>(OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        return _innerEmit.CreateDelegate<TDelegateType>(optimizationOptions);
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CreateMethod" />
    public MethodBuilder CreateMethod()
    {
        return _innerEmit.CreateMethod();
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CreateConstructor" />
    public ConstructorBuilder CreateConstructor()
    {
        return _innerEmit.CreateConstructor();
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Add" />
    public EmitShorthand Add()
    {
        _innerEmit.Add();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.AddOverflow" />
    public EmitShorthand Add_Ovf()
    {
        _innerEmit.AddOverflow();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedAddOverflow" />
    public EmitShorthand Add_Ovf_Un()
    {
        _innerEmit.UnsignedAddOverflow();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.And" />
    public EmitShorthand And()
    {
        _innerEmit.And();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfEqual(Sigil.Label)" />
    public EmitShorthand Beq(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfEqual(System.String)" />
    public EmitShorthand Beq(string name)
    {
        _innerEmit.BranchIfEqual(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfGreaterOrEqual(Sigil.Label)" />
    public EmitShorthand Bge(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfGreaterOrEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfGreaterOrEqual(System.String)" />
    public EmitShorthand Bge(string name)
    {
        _innerEmit.BranchIfGreaterOrEqual(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfGreaterOrEqual(Sigil.Label)" />
    public EmitShorthand Bge_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfGreaterOrEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfGreaterOrEqual(System.String)" />
    public EmitShorthand Bge_Un(string name)
    {
        _innerEmit.UnsignedBranchIfGreaterOrEqual(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfGreater(Sigil.Label)" />
    public EmitShorthand Bgt(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfGreater(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfGreater(System.String)" />
    public EmitShorthand Bgt(string name)
    {
        _innerEmit.BranchIfGreater(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfGreater(Sigil.Label)" />
    public EmitShorthand Bgt_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfGreater(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfGreater(System.String)" />
    public EmitShorthand Bgt_Un(string name)
    {
        _innerEmit.UnsignedBranchIfGreater(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfLessOrEqual(Sigil.Label)" />
    public EmitShorthand Ble(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfLessOrEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfLessOrEqual(System.String)" />
    public EmitShorthand Ble(string name)
    {
        _innerEmit.BranchIfLessOrEqual(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfLessOrEqual(Sigil.Label)" />
    public EmitShorthand Ble_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfLessOrEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfLessOrEqual(System.String)" />
    public EmitShorthand Ble_Un(string name)
    {
        _innerEmit.UnsignedBranchIfLessOrEqual(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfLess(Sigil.Label)" />
    public EmitShorthand Blt(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfLess(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfLess(System.String)" />
    public EmitShorthand Blt(string name)
    {
        _innerEmit.BranchIfLess(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfLess(Sigil.Label)" />
    public EmitShorthand Blt_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfLess(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfLess(System.String)" />
    public EmitShorthand Blt_Un(string name)
    {
        _innerEmit.UnsignedBranchIfLess(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfNotEqual(Sigil.Label)" />
    public EmitShorthand Bne_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfNotEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedBranchIfNotEqual(System.String)" />
    public EmitShorthand Bne_Un(string name)
    {
        _innerEmit.UnsignedBranchIfNotEqual(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Box``1()" />
    public EmitShorthand Box<TValueType>()
    {
        Box(typeof(TValueType));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Box(System.Type)" />
    public EmitShorthand Box(Type valueType)
    {
        _innerEmit.Box(valueType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Branch(Sigil.Label)" />
    public EmitShorthand Br(SigilLabel sigilLabel)
    {
        _innerEmit.Branch(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Branch(System.String)" />
    public EmitShorthand Br(string name)
    {
        _innerEmit.Branch(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Break" />
    public EmitShorthand Break()
    {
        _innerEmit.Break();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfFalse(Sigil.Label)" />
    public EmitShorthand Brfalse(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfFalse(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfFalse(System.String)" />
    public EmitShorthand Brfalse(string name)
    {
        _innerEmit.BranchIfFalse(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfTrue(Sigil.Label)" />
    public EmitShorthand Brtrue(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfTrue(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.BranchIfTrue(System.String)" />
    public EmitShorthand Brtrue(string name)
    {
        _innerEmit.BranchIfTrue(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Call(System.Reflection.MethodInfo)" />
    public EmitShorthand Call(MethodInfo method)
    {
        _innerEmit.Call(method);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CallIndirect(System.Reflection.CallingConventions,System.Type,System.Type[])" />
    public EmitShorthand Calli(CallingConventions callingConvention, Type returnType, params Type[] parameterTypes)
    {
        _innerEmit.CallIndirect(callingConvention, returnType, parameterTypes);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CallVirtual(System.Reflection.MethodInfo, System.Type)" />
    public EmitShorthand Callvirt(MethodInfo method, Type constrained = null)
    {
        _innerEmit.CallVirtual(method, constrained);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CastClass``1" />
    public EmitShorthand Castclass<TReferenceType>()
    {
        Castclass(typeof(TReferenceType));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CastClass(System.Type)" />
    public EmitShorthand Castclass(Type referenceType)
    {
        _innerEmit.CastClass(referenceType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CompareEqual" />
    public EmitShorthand Ceq()
    {
        _innerEmit.CompareEqual();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CompareGreaterThan" />
    public EmitShorthand Cgt()
    {
        _innerEmit.CompareGreaterThan();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedCompareGreaterThan" />
    public EmitShorthand Cgt_Un()
    {
        _innerEmit.UnsignedCompareGreaterThan();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CheckFinite" />
    public EmitShorthand Ckfinite()
    {
        _innerEmit.CheckFinite();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CompareLessThan" />
    public EmitShorthand Clt()
    {
        _innerEmit.CompareLessThan();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedCompareLessThan" />
    public EmitShorthand Clt_Un()
    {
        _innerEmit.UnsignedCompareLessThan();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedConvertOverflow(System.Type)" />
    public EmitShorthand Conv_Ovf_Un<TPrimitiveType>()
    {
        Conv_Ovf_Un(typeof(TPrimitiveType));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedConvertOverflow(System.Type)" />
    public EmitShorthand Conv_Ovf_Un(Type primitiveType)
    {
        _innerEmit.UnsignedConvertOverflow(primitiveType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedConvertToFloat" />
    public EmitShorthand Conv_R_Un()
    {
        _innerEmit.UnsignedConvertToFloat();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Convert(System.Type)" />
    public EmitShorthand Conv<TPrimitiveType>()
    {
        Conv(typeof(TPrimitiveType));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Convert(System.Type)" />
    public EmitShorthand Conv(Type primitiveType)
    {
        _innerEmit.Convert(primitiveType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ConvertOverflow(System.Type)" />
    public EmitShorthand Conv_Ovf<TPrimitType>()
    {
        Conv_Ovf(typeof(TPrimitType));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ConvertOverflow(System.Type)" />
    public EmitShorthand Conv_Ovf(Type primitiveType)
    {
        _innerEmit.ConvertOverflow(primitiveType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CopyBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Cpblk(bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.CopyBlock(isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CopyObject(System.Type)" />
    public EmitShorthand Cpobj<TValueType>()
    {
        Cpobj(typeof(TValueType));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.CopyObject(System.Type)" />
    public EmitShorthand Cpobj(Type valueType)
    {
        _innerEmit.CopyObject(valueType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Divide" />
    public EmitShorthand Div()
    {
        _innerEmit.Divide();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedDivide" />
    public EmitShorthand Div_Un()
    {
        _innerEmit.UnsignedDivide();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Duplicate" />
    public EmitShorthand Dup()
    {
        _innerEmit.Duplicate();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.InitializeBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Initblk(bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.InitializeBlock(isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.InitializeObject(System.Type)" />
    public EmitShorthand Initobj<TValueType>()
    {
        Initobj(typeof(TValueType));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.InitializeObject(System.Type)" />
    public EmitShorthand Initobj(Type valueType)
    {
        _innerEmit.InitializeObject(valueType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.IsInstance(System.Type)" />
    public EmitShorthand Isinst<TYpe>()
    {
        Isinst(typeof(TYpe));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.IsInstance(System.Type)" />
    public EmitShorthand Isinst(Type type)
    {
        _innerEmit.IsInstance(type);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Jump(System.Reflection.MethodInfo)" />
    public EmitShorthand Jmp(MethodInfo method)
    {
        _innerEmit.Jump(method);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadArgument(System.Int32)" />
    public EmitShorthand Ldarg(ushort index)
    {
        _innerEmit.LoadArgument(index);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadArgumentAddress(System.Int32)" />
    public EmitShorthand Ldarga(ushort index)
    {
        _innerEmit.LoadArgumentAddress(index);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Boolean)" />
    public EmitShorthand Ldc(bool b)
    {
        _innerEmit.LoadConstant(b);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Single)" />
    public EmitShorthand Ldc(float f)
    {
        _innerEmit.LoadConstant(f);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Double)" />
    public EmitShorthand Ldc(double d)
    {
        _innerEmit.LoadConstant(d);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.UInt32)" />
    public EmitShorthand Ldc(uint u)
    {
        _innerEmit.LoadConstant(u);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Int32)" />
    public EmitShorthand Ldc(int i)
    {
        _innerEmit.LoadConstant(i);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Int64)" />
    public EmitShorthand Ldc(long l)
    {
        _innerEmit.LoadConstant(l);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.UInt64)" />
    public EmitShorthand Ldc(ulong u)
    {
        _innerEmit.LoadConstant(u);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadElement``1" />
    public EmitShorthand Ldelem<TElementType>()
    {
        return Ldelem(typeof(TElementType));
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadElement(System.Type)" />
    public EmitShorthand Ldelem(Type elementType)
    {
        _innerEmit.LoadElement(elementType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadElementAddress``1" />
    public EmitShorthand Ldelema<TElementType>()
    {
        return Ldelema(typeof(TElementType));
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadElementAddress(System.Type)" />
    public EmitShorthand Ldelema(Type elementType)
    {
        _innerEmit.LoadElementAddress(elementType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Ldfld(FieldInfo field, bool? isVolatile = null, int? unaligned = null)
    {
        _innerEmit.LoadField(field, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadFieldAddress(System.Reflection.FieldInfo)" />
    public EmitShorthand Ldflda(FieldInfo field)
    {
        _innerEmit.LoadFieldAddress(field);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadFunctionPointer(System.Reflection.MethodInfo)" />
    public EmitShorthand Ldftn(MethodInfo method)
    {
        _innerEmit.LoadFunctionPointer(method);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Ldind<TYpe>(bool isVolatile = false, int? unaligned = null)
    {
        Ldind(typeof(TYpe), isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Ldind(Type type, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.LoadIndirect(type, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLength``1" />
    public EmitShorthand Ldlen<TElementType>()
    {
        return Ldlen(typeof(TElementType));
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLength(System.Type)" />
    public EmitShorthand Ldlen(Type elementType)
    {
        _innerEmit.LoadLength(elementType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLocal(Sigil.Local)" />
    public EmitShorthand Ldloc(SigilLocal sigilLocal)
    {
        _innerEmit.LoadLocal(sigilLocal);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLocal(System.String)" />
    public EmitShorthand Ldloc(string name)
    {
        _innerEmit.LoadLocal(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLocalAddress(Sigil.Local)" />
    public EmitShorthand Ldloca(SigilLocal sigilLocal)
    {
        _innerEmit.LoadLocalAddress(sigilLocal);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadLocalAddress(System.String)" />
    public EmitShorthand Ldloca(string name)
    {
        _innerEmit.LoadLocalAddress(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadNull" />
    public EmitShorthand Ldnull()
    {
        _innerEmit.LoadNull();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Ldobj<TValueType>(bool isVolatile = false, int? unaligned = null)
    {
        Ldobj(typeof(TValueType), isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Ldobj(Type valueType, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.LoadObject(valueType, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.String)" />
    public EmitShorthand Ldstr(string str)
    {
        _innerEmit.LoadConstant(str);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Reflection.FieldInfo)" />
    public EmitShorthand Ldtoken(FieldInfo field)
    {
        _innerEmit.LoadConstant(field);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Reflection.MethodInfo)" />
    public EmitShorthand Ldtoken(MethodInfo method)
    {
        _innerEmit.LoadConstant(method);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Type)" />
    public EmitShorthand Ldtoken<TYpe>()
    {
        Ldtoken(typeof(TYpe));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadConstant(System.Type)" />
    public EmitShorthand Ldtoken(Type type)
    {
        _innerEmit.LoadConstant(type);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LoadVirtualFunctionPointer(System.Reflection.MethodInfo)" />
    public EmitShorthand Ldvirtftn(MethodInfo method)
    {
        _innerEmit.LoadVirtualFunctionPointer(method);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Leave(Sigil.Label)" />
    public EmitShorthand Leave(SigilLabel sigilLabel)
    {
        _innerEmit.Leave(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Leave(System.String)" />
    public EmitShorthand Leave(string name)
    {
        _innerEmit.Leave(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.LocalAllocate" />
    public EmitShorthand Localloc()
    {
        _innerEmit.LocalAllocate();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Multiply" />
    public EmitShorthand Mul()
    {
        _innerEmit.Multiply();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.MultiplyOverflow" />
    public EmitShorthand Mul_Ovf()
    {
        _innerEmit.MultiplyOverflow();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedMultiplyOverflow" />
    public EmitShorthand Mul_Ovf_Un()
    {
        _innerEmit.UnsignedMultiplyOverflow();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Negate" />
    public EmitShorthand Neg()
    {
        _innerEmit.Negate();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.NewArray(System.Type)" />
    public EmitShorthand Newarr<TElementType>()
    {
        Newarr(typeof(TElementType));

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.NewArray(System.Type)" />
    public EmitShorthand Newarr(Type elementType)
    {
        _innerEmit.NewArray(elementType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.NewObject(System.Reflection.ConstructorInfo)" />
    public EmitShorthand Newobj(ConstructorInfo constructor)
    {
        _innerEmit.NewObject(constructor);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Nop" />
    public EmitShorthand Nop()
    {
        _innerEmit.Nop();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Not" />
    public EmitShorthand Not()
    {
        _innerEmit.Not();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Or" />
    public EmitShorthand Or()
    {
        _innerEmit.Or();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Pop" />
    public EmitShorthand Pop()
    {
        _innerEmit.Pop();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Remainder" />
    public EmitShorthand Rem()
    {
        _innerEmit.Remainder();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedRemainder" />
    public EmitShorthand Rem_Un()
    {
        _innerEmit.UnsignedRemainder();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Return" />
    public EmitShorthand Ret()
    {
        _innerEmit.Return();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ReThrow" />
    public EmitShorthand Rethrow()
    {
        _innerEmit.ReThrow();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ShiftLeft" />
    public EmitShorthand Shl()
    {
        _innerEmit.ShiftLeft();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.ShiftRight" />
    public EmitShorthand Shr()
    {
        _innerEmit.ShiftRight();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedShiftRight" />
    public EmitShorthand Shr_Un()
    {
        _innerEmit.UnsignedShiftRight();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.SizeOf(System.Type)" />
    public EmitShorthand Sizeof<TValueType>()
    {
        return Sizeof(typeof(TValueType));
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.SizeOf(System.Type)" />
    public EmitShorthand Sizeof(Type valueType)
    {
        _innerEmit.SizeOf(valueType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreArgument(System.Int32)" />
    public EmitShorthand Starg(ushort index)
    {
        _innerEmit.StoreArgument(index);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreElement``1" />
    public EmitShorthand Stelem<TElementType>()
    {
        return Stelem(typeof(TElementType));
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreElement(System.Type)" />
    public EmitShorthand Stelem(Type elementType)
    {
        _innerEmit.StoreElement(elementType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Stfld(FieldInfo field, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.StoreField(field, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Stind<TYpe>(bool isVolatile = false, int? unaligned = null)
    {
        return Stind(typeof(TYpe), isVolatile, unaligned);
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Stind(Type type, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.StoreIndirect(type, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreLocal(Sigil.Local)" />
    public EmitShorthand Stloc(SigilLocal sigilLocal)
    {
        _innerEmit.StoreLocal(sigilLocal);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreLocal(System.String)" />
    public EmitShorthand Stloc(string name)
    {
        _innerEmit.StoreLocal(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Stobj<TValueType>(bool isVolatile = false, int? unaligned = null)
    {
        return Stobj(typeof(TValueType), isVolatile, unaligned);
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand Stobj(Type valueType, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.StoreObject(valueType, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Subtract" />
    public EmitShorthand Sub()
    {
        _innerEmit.Subtract();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.SubtractOverflow" />
    public EmitShorthand Sub_Ovf()
    {
        _innerEmit.SubtractOverflow();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnsignedSubtractOverflow" />
    public EmitShorthand Sub_Ovf_Un()
    {
        _innerEmit.UnsignedSubtractOverflow();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Switch(Sigil.Label[])" />
    public EmitShorthand Switch(params SigilLabel[] labels)
    {
        _innerEmit.Switch(labels);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Switch(System.String[])" />
    public EmitShorthand Switch(params string[] names)
    {
        _innerEmit.Switch(names);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Throw" />
    public EmitShorthand Throw()
    {
        _innerEmit.Throw();

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Unbox(System.Type)" />
    public EmitShorthand Unbox<TValueType>()
    {
        return Unbox(typeof(TValueType));
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Unbox(System.Type)" />
    public EmitShorthand Unbox(Type valueType)
    {
        _innerEmit.Unbox(valueType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnboxAny(System.Type)" />
    public EmitShorthand Unbox_Any<TValueType>()
    {
        return Unbox_Any(typeof(TValueType));
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.UnboxAny(System.Type)" />
    public EmitShorthand Unbox_Any(Type valueType)
    {
        _innerEmit.UnboxAny(valueType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.WriteLine(System.String)" />
    public EmitShorthand WriteLine(string line, params SigilLocal[] locals)
    {
        _innerEmit.WriteLine(line, locals);

        return this;
    }

    /// <summary cref="M:Sigil.Emit.NonGeneric.Emit`1.Xor" />
    public EmitShorthand Xor()
    {
        _innerEmit.Xor();

        return this;
    }
}
