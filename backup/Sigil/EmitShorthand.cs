using ScrubJay.Sigil.Emission.Generic;

namespace ScrubJay.Sigil;

/// <summary>
/// A version of Emit with shorter named versions of it's methods.
///
/// Method names map more or less to OpCodes fields.
/// </summary>
public class EmitShorthand<TDelegateType>
{
    private readonly Emit<TDelegateType> _innerEmit;

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

    internal EmitShorthand(Emit<TDelegateType> inner)
    {
        _innerEmit = inner;
    }

    /// <summary>
    /// Returns the original Emit instance that AsShorthand() was called on.
    /// </summary>
    public Emit<TDelegateType> AsLonghand()
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

    /// <summary cref="M:Sigil.Emit`1.DeclareLocal``1(System.String, System.Boolean)" />
    public SigilLocal DeclareLocal<TYpe>(string name = null, bool initializeReused = true)
    {
        return DeclareLocal(typeof(TYpe), name, initializeReused);
    }

    /// <summary cref="M:Sigil.Emit`1.DeclareLocal``1(Sigil.Local, System.String, System.Boolean)" />
    public EmitShorthand<TDelegateType> DeclareLocal<TYpe>(out SigilLocal sigilLocal, string name = null, bool initializeReused = true)
    {
        sigilLocal = DeclareLocal<TYpe>(name, initializeReused);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.DeclareLocal(System.Type, System.String, System.Boolean)" />
    public SigilLocal DeclareLocal(Type type, string name = null, bool initializeReused = true)
    {
        return _innerEmit.DeclareLocal(type, name, initializeReused);
    }

    /// <summary cref="M:Sigil.Emit`1.DeclareLocal(System.Type, Sigil.Local, System.String, System.Boolean)" />
    public EmitShorthand<TDelegateType> DeclareLocal(Type type, out SigilLocal sigilLocal, string name = null, bool initializeReused = true)
    {
        sigilLocal = DeclareLocal(type, name, initializeReused);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.DefineLabel(System.String)" />
    public SigilLabel DefineLabel(string name = null)
    {
        return _innerEmit.DefineLabel(name);
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.DefineLabel(System.String)" />
    public EmitShorthand<TDelegateType> DefineLabel(out SigilLabel sigilLabel, string name = null)
    {
        sigilLabel = DefineLabel(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.MarkLabel(Sigil.Label, IEnumerable``1)" />
    public EmitShorthand<TDelegateType> MarkLabel(SigilLabel sigilLabel)
    {
        _innerEmit.MarkLabel(sigilLabel);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.MarkLabel(System.String, IEnumerable``1)" />
    public EmitShorthand<TDelegateType> MarkLabel(string name)
    {
        _innerEmit.MarkLabel(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BeginExceptionBlock" />
    public ExceptionBlock BeginExceptionBlock()
    {
        return _innerEmit.BeginExceptionBlock();
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BeginExceptionBlock" />
    public EmitShorthand<TDelegateType> BeginExceptionBlock(out ExceptionBlock forTry)
    {
        forTry = BeginExceptionBlock();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BeginCatchBlock``1(ScrubJay.Sigil.ExceptionBlock)" />
    public CatchBlock BeginCatchBlock<TExceptionType>(ExceptionBlock forTry)
    {
        return BeginCatchBlock(forTry, typeof(TExceptionType));
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BeginCatchBlock``1(ScrubJay.Sigil.ExceptionBlock)" />
    public EmitShorthand<TDelegateType> BeginCatchBlock<TExceptionType>(ExceptionBlock forTry, out CatchBlock tryCatch)
    {
        tryCatch = BeginCatchBlock<TExceptionType>(forTry);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.BeginCatchBlock(System.Type, Sigil.ExceptionBlock)" />
    public CatchBlock BeginCatchBlock(ExceptionBlock forTry, Type exceptionType)
    {
        return _innerEmit.BeginCatchBlock(forTry, exceptionType);
    }

    /// <summary cref="M:Sigil.Emit`1.BeginCatchBlock(System.Type, Sigil.ExceptionBlock)" />
    public EmitShorthand<TDelegateType> BeginCatchBlock(ExceptionBlock forTry, Type exceptionType, out CatchBlock forCatch)
    {
        forCatch = BeginCatchBlock(forTry, exceptionType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.EndCatchBlock(ScrubJay.Sigil.CatchBlock)" />
    public EmitShorthand<TDelegateType> EndCatchBlock(CatchBlock forCatch)
    {
        _innerEmit.EndCatchBlock(forCatch);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BeginFinallyBlock(ScrubJay.Sigil.ExceptionBlock)" />
    public FinallyBlock BeginFinallyBlock(ExceptionBlock forTry)
    {
        return _innerEmit.BeginFinallyBlock(forTry);
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BeginFinallyBlock(ScrubJay.Sigil.ExceptionBlock)" />
    public EmitShorthand<TDelegateType> BeginFinallyBlock(ExceptionBlock forTry, out FinallyBlock forFinally)
    {
        forFinally = BeginFinallyBlock(forTry);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.EndFinallyBlock(ScrubJay.Sigil.FinallyBlock)" />
    public EmitShorthand<TDelegateType> EndFinallyBlock(FinallyBlock forFinally)
    {
        _innerEmit.EndFinallyBlock(forFinally);
        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.EndExceptionBlock(ScrubJay.Sigil.ExceptionBlock)" />
    public EmitShorthand<TDelegateType> EndExceptionBlock(ExceptionBlock forTry)
    {
        _innerEmit.EndExceptionBlock(forTry);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CreateDelegate(ScrubJay.Sigil.OptimizationOptions)" />
    public TDelegateType CreateDelegate(OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        return _innerEmit.CreateDelegate(optimizationOptions);
    }

    /// <summary cref="M:Sigil.Emit`1.CreateMethod" />
    public MethodBuilder CreateMethod()
    {
        return _innerEmit.CreateMethod();
    }

    /// <summary cref="M:Sigil.Emit`1.CreateConstructor" />
    public ConstructorBuilder CreateConstructor()
    {
        return _innerEmit.CreateConstructor();
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Add" />
    public EmitShorthand<TDelegateType> Add()
    {
        _innerEmit.Add();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.AddOverflow" />
    public EmitShorthand<TDelegateType> Add_Ovf()
    {
        _innerEmit.AddOverflow();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedAddOverflow" />
    public EmitShorthand<TDelegateType> Add_Ovf_Un()
    {
        _innerEmit.UnsignedAddOverflow();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.And" />
    public EmitShorthand<TDelegateType> And()
    {
        _innerEmit.And();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfEqual(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Beq(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfEqual(System.String)" />
    public EmitShorthand<TDelegateType> Beq(string name)
    {
        _innerEmit.BranchIfEqual(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfGreaterOrEqual(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Bge(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfGreaterOrEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfGreaterOrEqual(System.String)" />
    public EmitShorthand<TDelegateType> Bge(string name)
    {
        _innerEmit.BranchIfGreaterOrEqual(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfGreaterOrEqual(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Bge_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfGreaterOrEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfGreaterOrEqual(System.String)" />
    public EmitShorthand<TDelegateType> Bge_Un(string name)
    {
        _innerEmit.UnsignedBranchIfGreaterOrEqual(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfGreater(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Bgt(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfGreater(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfGreater(System.String)" />
    public EmitShorthand<TDelegateType> Bgt(string name)
    {
        _innerEmit.BranchIfGreater(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfGreater(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Bgt_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfGreater(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfGreater(System.String)" />
    public EmitShorthand<TDelegateType> Bgt_Un(string name)
    {
        _innerEmit.UnsignedBranchIfGreater(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfLessOrEqual(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Ble(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfLessOrEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfLessOrEqual(System.String)" />
    public EmitShorthand<TDelegateType> Ble(string name)
    {
        _innerEmit.BranchIfLessOrEqual(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfLessOrEqual(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Ble_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfLessOrEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfLessOrEqual(System.String)" />
    public EmitShorthand<TDelegateType> Ble_Un(string name)
    {
        _innerEmit.UnsignedBranchIfLessOrEqual(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfLess(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Blt(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfLess(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfLess(System.String)" />
    public EmitShorthand<TDelegateType> Blt(string name)
    {
        _innerEmit.BranchIfLess(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfLess(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Blt_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfLess(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfLess(System.String)" />
    public EmitShorthand<TDelegateType> Blt_Un(string name)
    {
        _innerEmit.UnsignedBranchIfLess(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfNotEqual(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Bne_Un(SigilLabel sigilLabel)
    {
        _innerEmit.UnsignedBranchIfNotEqual(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedBranchIfNotEqual(System.String)" />
    public EmitShorthand<TDelegateType> Bne_Un(string name)
    {
        _innerEmit.UnsignedBranchIfNotEqual(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.Box``1()" />
    public EmitShorthand<TDelegateType> Box<TValueType>()
    {
        Box(typeof(TValueType));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Box(System.Type)" />
    public EmitShorthand<TDelegateType> Box(Type valueType)
    {
        _innerEmit.Box(valueType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Branch(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Br(SigilLabel sigilLabel)
    {
        _innerEmit.Branch(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Branch(System.String)" />
    public EmitShorthand<TDelegateType> Br(string name)
    {
        _innerEmit.Branch(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Break" />
    public EmitShorthand<TDelegateType> Break()
    {
        _innerEmit.Break();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfFalse(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Brfalse(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfFalse(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfFalse(System.String)" />
    public EmitShorthand<TDelegateType> Brfalse(string name)
    {
        _innerEmit.BranchIfFalse(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfTrue(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Brtrue(SigilLabel sigilLabel)
    {
        _innerEmit.BranchIfTrue(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.BranchIfTrue(System.String)" />
    public EmitShorthand<TDelegateType> Brtrue(string name)
    {
        _innerEmit.BranchIfTrue(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.Call(System.Reflection.MethodInfo)" />
    public EmitShorthand<TDelegateType> Call(MethodInfo method)
    {
        _innerEmit.Call(method);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CallIndirect(System.Reflection.CallingConventions,System.Type,System.Type[])" />
    public EmitShorthand<TDelegateType> Calli(CallingConventions callingConvention, Type returnType, params Type[] parameterTypes)
    {
        _innerEmit.CallIndirect(callingConvention, returnType, parameterTypes);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.CallVirtual(System.Reflection.MethodInfo, System.Type)" />
    public EmitShorthand<TDelegateType> Callvirt(MethodInfo method, Type constrained = null)
    {
        _innerEmit.CallVirtual(method, constrained);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CastClass``1" />
    public EmitShorthand<TDelegateType> Castclass<TReferenceType>()
    {
        Castclass(typeof(TReferenceType));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CastClass(System.Type)" />
    public EmitShorthand<TDelegateType> Castclass(Type referenceType)
    {
        _innerEmit.CastClass(referenceType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CompareEqual" />
    public EmitShorthand<TDelegateType> Ceq()
    {
        _innerEmit.CompareEqual();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CompareGreaterThan" />
    public EmitShorthand<TDelegateType> Cgt()
    {
        _innerEmit.CompareGreaterThan();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedCompareGreaterThan" />
    public EmitShorthand<TDelegateType> Cgt_Un()
    {
        _innerEmit.UnsignedCompareGreaterThan();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CheckFinite" />
    public EmitShorthand<TDelegateType> Ckfinite()
    {
        _innerEmit.CheckFinite();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CompareLessThan" />
    public EmitShorthand<TDelegateType> Clt()
    {
        _innerEmit.CompareLessThan();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedCompareLessThan" />
    public EmitShorthand<TDelegateType> Clt_Un()
    {
        _innerEmit.UnsignedCompareLessThan();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedConvertOverflow(System.Type)" />
    public EmitShorthand<TDelegateType> Conv_Ovf_Un<TPrimitiveType>()
    {
        Conv_Ovf_Un(typeof(TPrimitiveType));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedConvertOverflow(System.Type)" />
    public EmitShorthand<TDelegateType> Conv_Ovf_Un(Type primitiveType)
    {
        _innerEmit.UnsignedConvertOverflow(primitiveType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedConvertToFloat" />
    public EmitShorthand<TDelegateType> Conv_R_Un()
    {
        _innerEmit.UnsignedConvertToFloat();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Convert(System.Type)" />
    public EmitShorthand<TDelegateType> Conv<TPrimitiveType>()
    {
        Conv(typeof(TPrimitiveType));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Convert(System.Type)" />
    public EmitShorthand<TDelegateType> Conv(Type primitiveType)
    {
        _innerEmit.Convert(primitiveType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.ConvertOverflow(System.Type)" />
    public EmitShorthand<TDelegateType> Conv_Ovf<TPrimitType>()
    {
        Conv_Ovf(typeof(TPrimitType));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.ConvertOverflow(System.Type)" />
    public EmitShorthand<TDelegateType> Conv_Ovf(Type primitiveType)
    {
        _innerEmit.ConvertOverflow(primitiveType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.CopyBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Cpblk(bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.CopyBlock(isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CopyObject(System.Type)" />
    public EmitShorthand<TDelegateType> Cpobj<TValueType>()
    {
        Cpobj(typeof(TValueType));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.CopyObject(System.Type)" />
    public EmitShorthand<TDelegateType> Cpobj(Type valueType)
    {
        _innerEmit.CopyObject(valueType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Divide" />
    public EmitShorthand<TDelegateType> Div()
    {
        _innerEmit.Divide();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedDivide" />
    public EmitShorthand<TDelegateType> Div_Un()
    {
        _innerEmit.UnsignedDivide();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Duplicate" />
    public EmitShorthand<TDelegateType> Dup()
    {
        _innerEmit.Duplicate();

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.InitializeBlock(System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Initblk(bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.InitializeBlock(isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.InitializeObject(System.Type)" />
    public EmitShorthand<TDelegateType> Initobj<TValueType>()
    {
        Initobj(typeof(TValueType));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.InitializeObject(System.Type)" />
    public EmitShorthand<TDelegateType> Initobj(Type valueType)
    {
        _innerEmit.InitializeObject(valueType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.IsInstance(System.Type)" />
    public EmitShorthand<TDelegateType> Isinst<TYpe>()
    {
        Isinst(typeof(TYpe));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.IsInstance(System.Type)" />
    public EmitShorthand<TDelegateType> Isinst(Type type)
    {
        _innerEmit.IsInstance(type);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Jump(System.Reflection.MethodInfo)" />
    public EmitShorthand<TDelegateType> Jmp(MethodInfo method)
    {
        _innerEmit.Jump(method);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.LoadArgument(System.Int32)" />
    public EmitShorthand<TDelegateType> Ldarg(ushort index)
    {
        _innerEmit.LoadArgument(index);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.LoadArgumentAddress(System.Int32)" />
    public EmitShorthand<TDelegateType> Ldarga(ushort index)
    {
        _innerEmit.LoadArgumentAddress(index);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.Boolean)" />
    public EmitShorthand<TDelegateType> Ldc(bool b)
    {
        _innerEmit.LoadConstant(b);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.Single)" />
    public EmitShorthand<TDelegateType> Ldc(float f)
    {
        _innerEmit.LoadConstant(f);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.Double)" />
    public EmitShorthand<TDelegateType> Ldc(double d)
    {
        _innerEmit.LoadConstant(d);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.UInt32)" />
    public EmitShorthand<TDelegateType> Ldc(uint u)
    {
        _innerEmit.LoadConstant(u);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.Int32)" />
    public EmitShorthand<TDelegateType> Ldc(int i)
    {
        _innerEmit.LoadConstant(i);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.Int64)" />
    public EmitShorthand<TDelegateType> Ldc(long l)
    {
        _innerEmit.LoadConstant(l);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.UInt64)" />
    public EmitShorthand<TDelegateType> Ldc(ulong u)
    {
        _innerEmit.LoadConstant(u);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadElement``1" />
    public EmitShorthand<TDelegateType> Ldelem<TElementType>()
    {
        return Ldelem(typeof(TElementType));
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadElement(System.Type)" />
    public EmitShorthand<TDelegateType> Ldelem(Type elementType)
    {
        _innerEmit.LoadElement(elementType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadElementAddress``1" />
    public EmitShorthand<TDelegateType> Ldelema<TElementType>()
    {
        return Ldelema(typeof(TElementType));
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadElementAddress(System.Type)" />
    public EmitShorthand<TDelegateType> Ldelema(Type elementType)
    {
        _innerEmit.LoadElementAddress(elementType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.LoadField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Ldfld(FieldInfo field, bool? isVolatile = null, int? unaligned = null)
    {
        _innerEmit.LoadField(field, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadFieldAddress(System.Reflection.FieldInfo)" />
    public EmitShorthand<TDelegateType> Ldflda(FieldInfo field)
    {
        _innerEmit.LoadFieldAddress(field);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadFunctionPointer(System.Reflection.MethodInfo)" />
    public EmitShorthand<TDelegateType> Ldftn(MethodInfo method)
    {
        _innerEmit.LoadFunctionPointer(method);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Ldind<TYpe>(bool isVolatile = false, int? unaligned = null)
    {
        Ldind(typeof(TYpe), isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.LoadIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Ldind(Type type, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.LoadIndirect(type, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadLength``1" />
    public EmitShorthand<TDelegateType> Ldlen<TElementType>()
    {
        return Ldlen(typeof(TElementType));
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadLength(System.Type)" />
    public EmitShorthand<TDelegateType> Ldlen(Type elementType)
    {
        _innerEmit.LoadLength(elementType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadLocal(ScrubJay.Sigil.Local)" />
    public EmitShorthand<TDelegateType> Ldloc(SigilLocal sigilLocal)
    {
        _innerEmit.LoadLocal(sigilLocal);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadLocal(System.String)" />
    public EmitShorthand<TDelegateType> Ldloc(string name)
    {
        _innerEmit.LoadLocal(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadLocalAddress(ScrubJay.Sigil.Local)" />
    public EmitShorthand<TDelegateType> Ldloca(SigilLocal sigilLocal)
    {
        _innerEmit.LoadLocalAddress(sigilLocal);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadLocalAddress(System.String)" />
    public EmitShorthand<TDelegateType> Ldloca(string name)
    {
        _innerEmit.LoadLocalAddress(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadNull" />
    public EmitShorthand<TDelegateType> Ldnull()
    {
        _innerEmit.LoadNull();

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Ldobj<TValueType>(bool isVolatile = false, int? unaligned = null)
    {
        Ldobj(typeof(TValueType), isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.LoadObject(System.Type, System.Boolen, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Ldobj(Type valueType, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.LoadObject(valueType, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.String)" />
    public EmitShorthand<TDelegateType> Ldstr(string str)
    {
        _innerEmit.LoadConstant(str);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.Reflection.FieldInfo)" />
    public EmitShorthand<TDelegateType> Ldtoken(FieldInfo field)
    {
        _innerEmit.LoadConstant(field);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.Reflection.MethodInfo)" />
    public EmitShorthand<TDelegateType> Ldtoken(MethodInfo method)
    {
        _innerEmit.LoadConstant(method);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.Type)" />
    public EmitShorthand<TDelegateType> Ldtoken<TYpe>()
    {
        Ldtoken(typeof(TYpe));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadConstant(System.Type)" />
    public EmitShorthand<TDelegateType> Ldtoken(Type type)
    {
        _innerEmit.LoadConstant(type);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LoadVirtualFunctionPointer(System.Reflection.MethodInfo)" />
    public EmitShorthand<TDelegateType> Ldvirtftn(MethodInfo method)
    {
        _innerEmit.LoadVirtualFunctionPointer(method);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Leave(ScrubJay.Sigil.Label)" />
    public EmitShorthand<TDelegateType> Leave(SigilLabel sigilLabel)
    {
        _innerEmit.Leave(sigilLabel);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Leave(System.String)" />
    public EmitShorthand<TDelegateType> Leave(string name)
    {
        _innerEmit.Leave(name);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.LocalAllocate" />
    public EmitShorthand<TDelegateType> Localloc()
    {
        _innerEmit.LocalAllocate();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Multiply" />
    public EmitShorthand<TDelegateType> Mul()
    {
        _innerEmit.Multiply();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.MultiplyOverflow" />
    public EmitShorthand<TDelegateType> Mul_Ovf()
    {
        _innerEmit.MultiplyOverflow();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedMultiplyOverflow" />
    public EmitShorthand<TDelegateType> Mul_Ovf_Un()
    {
        _innerEmit.UnsignedMultiplyOverflow();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Negate" />
    public EmitShorthand<TDelegateType> Neg()
    {
        _innerEmit.Negate();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.NewArray(System.Type)" />
    public EmitShorthand<TDelegateType> Newarr<TElementType>()
    {
        Newarr(typeof(TElementType));

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.NewArray(System.Type)" />
    public EmitShorthand<TDelegateType> Newarr(Type elementType)
    {
        _innerEmit.NewArray(elementType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.NewObject(System.Reflection.ConstructorInfo)" />
    public EmitShorthand<TDelegateType> Newobj(ConstructorInfo constructor)
    {
        _innerEmit.NewObject(constructor);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Nop" />
    public EmitShorthand<TDelegateType> Nop()
    {
        _innerEmit.Nop();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Not" />
    public EmitShorthand<TDelegateType> Not()
    {
        _innerEmit.Not();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Or" />
    public EmitShorthand<TDelegateType> Or()
    {
        _innerEmit.Or();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Pop" />
    public EmitShorthand<TDelegateType> Pop()
    {
        _innerEmit.Pop();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Remainder" />
    public EmitShorthand<TDelegateType> Rem()
    {
        _innerEmit.Remainder();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedRemainder" />
    public EmitShorthand<TDelegateType> Rem_Un()
    {
        _innerEmit.UnsignedRemainder();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Return" />
    public EmitShorthand<TDelegateType> Ret()
    {
        _innerEmit.Return();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.ReThrow" />
    public EmitShorthand<TDelegateType> Rethrow()
    {
        _innerEmit.ReThrow();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.ShiftLeft" />
    public EmitShorthand<TDelegateType> Shl()
    {
        _innerEmit.ShiftLeft();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.ShiftRight" />
    public EmitShorthand<TDelegateType> Shr()
    {
        _innerEmit.ShiftRight();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedShiftRight" />
    public EmitShorthand<TDelegateType> Shr_Un()
    {
        _innerEmit.UnsignedShiftRight();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.SizeOf(System.Type)" />
    public EmitShorthand<TDelegateType> Sizeof<TValueType>()
    {
        return Sizeof(typeof(TValueType));
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.SizeOf(System.Type)" />
    public EmitShorthand<TDelegateType> Sizeof(Type valueType)
    {
        _innerEmit.SizeOf(valueType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.StoreArgument(System.Int32)" />
    public EmitShorthand<TDelegateType> Starg(ushort index)
    {
        _innerEmit.StoreArgument(index);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.StoreElement``1" />
    public EmitShorthand<TDelegateType> Stelem<TElementType>()
    {
        return Stelem(typeof(TElementType));
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.StoreElement(System.Type)" />
    public EmitShorthand<TDelegateType> Stelem(Type elementType)
    {
        _innerEmit.StoreElement(elementType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.StoreField(System.Reflection.FieldInfo, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Stfld(FieldInfo field, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.StoreField(field, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Stind<TYpe>(bool isVolatile = false, int? unaligned = null)
    {
        return Stind(typeof(TYpe), isVolatile, unaligned);
    }

    /// <summary cref="M:Sigil.Emit`1.StoreIndirect(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Stind(Type type, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.StoreIndirect(type, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.StoreLocal(ScrubJay.Sigil.Local)" />
    public EmitShorthand<TDelegateType> Stloc(SigilLocal sigilLocal)
    {
        _innerEmit.StoreLocal(sigilLocal);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.StoreLocal(System.String)" />
    public EmitShorthand<TDelegateType> Stloc(string name)
    {
        _innerEmit.StoreLocal(name);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Stobj<TValueType>(bool isVolatile = false, int? unaligned = null)
    {
        return Stobj(typeof(TValueType), isVolatile, unaligned);
    }

    /// <summary cref="M:Sigil.Emit`1.StoreObject(System.Type, System.Boolean, System.Nullable&lt;int&gt;)" />
    public EmitShorthand<TDelegateType> Stobj(Type valueType, bool isVolatile = false, int? unaligned = null)
    {
        _innerEmit.StoreObject(valueType, isVolatile, unaligned);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Subtract" />
    public EmitShorthand<TDelegateType> Sub()
    {
        _innerEmit.Subtract();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.SubtractOverflow" />
    public EmitShorthand<TDelegateType> Sub_Ovf()
    {
        _innerEmit.SubtractOverflow();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnsignedSubtractOverflow" />
    public EmitShorthand<TDelegateType> Sub_Ovf_Un()
    {
        _innerEmit.UnsignedSubtractOverflow();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Switch(ScrubJay.Sigil.Label[])" />
    public EmitShorthand<TDelegateType> Switch(params SigilLabel[] labels)
    {
        _innerEmit.Switch(labels);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Switch(System.String[])" />
    public EmitShorthand<TDelegateType> Switch(params string[] names)
    {
        _innerEmit.Switch(names);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Throw" />
    public EmitShorthand<TDelegateType> Throw()
    {
        _innerEmit.Throw();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Unbox(System.Type)" />
    public EmitShorthand<TDelegateType> Unbox<TValueType>()
    {
        return Unbox(typeof(TValueType));
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Unbox(System.Type)" />
    public EmitShorthand<TDelegateType> Unbox(Type valueType)
    {
        _innerEmit.Unbox(valueType);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnboxAny(System.Type)" />
    public EmitShorthand<TDelegateType> Unbox_Any<TValueType>()
    {
        return Unbox_Any(typeof(TValueType));
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.UnboxAny(System.Type)" />
    public EmitShorthand<TDelegateType> Unbox_Any(Type valueType)
    {
        _innerEmit.UnboxAny(valueType);

        return this;
    }

    /// <summary cref="M:Sigil.Emit`1.WriteLine(System.String)" />
    public EmitShorthand<TDelegateType> WriteLine(string line, params SigilLocal[] locals)
    {
        _innerEmit.WriteLine(line, locals);

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.Xor" />
    public EmitShorthand<TDelegateType> Xor()
    {
        _innerEmit.Xor();

        return this;
    }

    /// <summary cref="M:ScrubJay.Sigil.Emission.Generic.Emit`1.TraceOperationResultUsage" />
    public IEnumerable<OperationResultUsage<TDelegateType>> TraceOperationResultUsage()
    {
        return _innerEmit.TraceOperationResultUsage();
    }
}
