using ScrubJay.Sigil.Utilities;

namespace ScrubJay.Sigil.Emission.Generic;

/// <summary>
/// Helper for CIL generation that fails as soon as a sequence of instructions
/// can be shown to be invalid.
/// </summary>
/// <typeparam name="TDelegateType">The type of delegate being built</typeparam>
public partial class Emit<TDelegateType>
{
    internal static readonly ModuleBuilder _module;

    static Emit()
    {
#if NETSTANDARD
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Sigil.Emit.DynamicAssembly"), AssemblyBuilderAccess.Run);
#elif NETFRAMEWORK
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Sigil.Emit.DynamicAssembly"), AssemblyBuilderAccess.Run);
#else
        AssemblyBuilder asm = default!;
        throw new NotImplementedException();
#endif
        _module = asm.DefineDynamicModule("DynamicModule");
    }

    private bool _invalidated;

    private readonly BufferedILGenerator<TDelegateType> _il;
    private readonly TypeOnStack _returnType;
    private readonly Type[] _parameterTypes;
    private readonly CallingConventions _callingConventions;

    private readonly List<VerifiableTracker> _trackers;

    private ushort _nextLocalIndex = 0;

    private readonly List<SigilLocal> _allLocals;

    private readonly HashSet<SigilLocal> _unusedLocals;
    private readonly HashSet<SigilLabel> _unusedLabels;
    private readonly HashSet<SigilLabel> _unmarkedLabels;

    private readonly List<Tuple<OpCode, SigilLabel, int>> _branches;
    private readonly Dictionary<SigilLabel, int> _marks;
    private readonly List<int> _returns;
    private readonly List<int> _throws;

    private readonly Dictionary<int, Tuple<SigilLabel, UpdateOpCodeDelegate, OpCode>> _branchPatches;

    private readonly LinqStack<ExceptionBlock> _currentExceptionBlock;

    private readonly Dictionary<ExceptionBlock, Tuple<int, int>> _tryBlocks;
    private readonly Dictionary<CatchBlock, Tuple<int, int>> _catchBlocks;
    private readonly Dictionary<FinallyBlock, Tuple<int, int>> _finallyBlocks;

    private readonly List<Tuple<int, TypeOnStack>> _readonlyPatches;

    private readonly EmitShorthand<TDelegateType> _shorthand;

    // These can only ever be set if we're building a DynamicMethod
    private TDelegateType _createdDelegate;
    internal DynamicMethod DynMethod { get; set; }

    // These can only ever be set if we're building a MethodBuilder
    internal MethodBuilder MtdBuilder { get; set; }
    private bool _methodBuilt;

    internal ConstructorBuilder ConstrBuilder { get; set; }
    internal bool IsBuildingConstructor { get; set; }
    internal Type ConstructorDefinedInType { get; set; }
    private bool _constructorBuilt;

    /// <summary>
    /// Returns true if this Emit can make use of unverifiable instructions.
    /// </summary>
    public bool AllowsUnverifiableCIL { get; private set; }

    private int _maxStackSize;

    /// <summary>
    /// Returns the maxmimum number of items on the stack for the IL stream created with the current emit.
    ///
    /// This is not the maximum that *can be placed*, but the maximum that actually are.
    /// </summary>
    public int MaxStackSize
    {
        get
        {
            if (!_isVerifying)
            {
                throw new InvalidOperationException("MaxStackSize is not available on non-verifying Emits");
            }

            return _maxStackSize;
        }
        private set
        {
            _maxStackSize = value;
        }
    }

    private List<SigilLocal> FreedLocals { get; set; }

    private readonly Dictionary<string, SigilLocal> _currentLocals;

    /// <summary>
    /// Lookup for the locals currently in scope by name.
    ///
    /// Locals go out of scope when released (by calling Dispose() directly, or via using) and go into scope
    /// immediately after a DeclareLocal()
    /// </summary>
    public LocalLookup Locals { get; private set; }

    private readonly Dictionary<string, SigilLabel> _currentLabels;

    /// <summary>
    /// Lookup for declared labels by name.
    /// </summary>
    public LabelLookup Labels { get; private set; }

    private readonly RollingVerifier _currentVerifiers;

    private bool _mustMark;

    private readonly List<int> _elidableCasts;

    private readonly Dictionary<int, List<TypeOnStack>> _typesProducedAtIndex;

    private readonly bool _isVerifying;

    private readonly bool _usesStrictBranchVerification;

    private Emit(CallingConventions callConvention, Type returnType, Type[] parameterTypes, bool allowUnverifiable, bool doVerify, bool strictBranchVerification)
    {
        _callingConventions = callConvention;

        AllowsUnverifiableCIL = allowUnverifiable;

        _isVerifying = doVerify;
        _usesStrictBranchVerification = strictBranchVerification;

        _returnType = TypeOnStack.Get(returnType);
        _parameterTypes = parameterTypes;

        _il = new BufferedILGenerator<TDelegateType>();

        _trackers = new List<VerifiableTracker>();

        _allLocals = new List<SigilLocal>();

        _unusedLocals = new HashSet<SigilLocal>();
        _unusedLabels = new HashSet<SigilLabel>();
        _unmarkedLabels = new HashSet<SigilLabel>();

        _branches = new List<Tuple<OpCode, SigilLabel, int>>();
        _marks = new Dictionary<SigilLabel, int>();
        _returns = new List<int>();
        _throws = new List<int>();

        _branchPatches = new Dictionary<int, Tuple<SigilLabel, UpdateOpCodeDelegate, OpCode>>();

        _currentExceptionBlock = new LinqStack<ExceptionBlock>();

        _tryBlocks = new Dictionary<ExceptionBlock, Tuple<int, int>>();
        _catchBlocks = new Dictionary<CatchBlock, Tuple<int, int>>();
        _finallyBlocks = new Dictionary<FinallyBlock, Tuple<int, int>>();

        _readonlyPatches = new List<Tuple<int, TypeOnStack>>();

        _shorthand = new EmitShorthand<TDelegateType>(this);

        FreedLocals = new List<SigilLocal>();

        _currentLocals = new Dictionary<string, SigilLocal>();
        Locals = new LocalLookup(_currentLocals);

        _currentLabels = new Dictionary<string, SigilLabel>();
        Labels = new LabelLookup(_currentLabels);

        _elidableCasts = new List<int>();

        _typesProducedAtIndex = new Dictionary<int, List<TypeOnStack>>();

        var start = DefineLabel("__start");
        _currentVerifiers = _isVerifying ? new RollingVerifier(start, _usesStrictBranchVerification) : new RollingVerifierWithoutVerification(start);
        MarkLabel(start);
    }

    internal static Emit<NonGenericPlaceholderDelegate> MakeNonGenericEmit(CallingConventions callConvention, Type returnType, Type[] parameterTypes, bool allowUnverifiable, bool doVerify, bool strictBranchVerification)
    {
        return new Emit<NonGenericPlaceholderDelegate>(callConvention, returnType, parameterTypes, allowUnverifiable, doVerify, strictBranchVerification);
    }

    /// <summary>
    /// Returns a proxy for this Emit that exposes method names that more closely
    /// match the fields on System.Reflection.Emit.OpCodes.
    ///
    /// IF you're well versed in ILGenerator, the shorthand version may be easier to use.
    /// </summary>
    public EmitShorthand<TDelegateType> AsShorthand()
    {
        return _shorthand;
    }

    /// <summary>
    /// Returns a string representation of the CIL opcodes written to this Emit to date.
    ///
    /// This method is meant for debugging purposes only.
    /// </summary>
    public string Instructions()
    {
        var ret = new StringBuilder();

        foreach (var line in (_il.Instructions(_allLocals)).Skip(2))
        {
            ret.AppendLine(line);
        }

        return ret.ToString().Trim();
    }

    /// <summary>
    /// Returns the current instruction offset (effectively, the length of the CIL stream to date).
    ///
    /// This does not necessarily increase monotonically, as rewrites can cause it to shrink.
    ///
    /// Likewise the effect of any given call is not guaranteed to be the same under all circumstance, as current and future
    /// state may influence opcode choice.
    ///
    /// This method is meant for debugging purposes only.
    /// </summary>
    public int ILOffset()
    {
        return _il.ByteDistance(0, _il.Index);
    }

    private void Seal(OptimizationOptions optimizationOptions)
    {
        if ((optimizationOptions & ~OptimizationOptions.All) != 0)
        {
            throw new ArgumentException("optimizationOptions contained unknown flags, found " + optimizationOptions);
        }

        if ((optimizationOptions & OptimizationOptions.EnableTrivialCastEliding) != 0)
        {
            ElideCasts();
        }

        InjectTailCall();
        InjectReadOnly();

        if ((optimizationOptions & OptimizationOptions.EnableBranchPatching) != 0)
        {
            PatchBranches();
        }

        Validate();

        _invalidated = true;
    }

    /// <summary>
    /// Traces where the values produced by certain operations are used.
    ///
    /// For example:
    ///   ldc.i4 32
    ///   ldc.i4 64
    ///   add
    ///   ret
    ///
    /// Would be represented by a series of OperationResultUsage like so:
    ///   - (lcd.i4 32) -> add
    ///   - (ldc.i4 64) -> add
    ///   - (add) -> ret
    /// </summary>
    public IEnumerable<OperationResultUsage<TDelegateType>> TraceOperationResultUsage()
    {
        var ret = new List<OperationResultUsage<TDelegateType>>();

        foreach (var r in _typesProducedAtIndex)
        {
            var allUsage = new List<InstructionAndTransitions>(r.Value.SelectMany(k => k.UsedBy.Select(u => u.Item1)));

            var usedBy = new List<Operation<TDelegateType>>(allUsage.Select(u => _il._operations[u.InstructionIndex.Value]).Distinct());

            if (r.Key < _il._operations.Count)
            {
                var key = _il._operations[r.Key];

                if (key != null)
                {
                    ret.Add(new OperationResultUsage<TDelegateType>(key, usedBy, r.Value));
                }
            }
        }

        return ret;
    }

    internal Delegate InnerCreateDelegate(Type delegateType, out string instructions, OptimizationOptions optimizationOptions)
    {
        Seal(optimizationOptions);

        var il = DynMethod.GetILGenerator();
        instructions = _il.UnBuffer(il);

        AutoNamer.Release(this);

        return DynMethod.CreateDelegate(delegateType);
    }

    /// <summary>
    /// Converts the CIL stream into a delegate.
    ///
    /// Validation that cannot be run until a method is finished is run, and various instructions
    /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
    ///
    /// Once this method is called the Emit may no longer be modified.
    ///
    /// `instructions` will be set to a representation of the instructions making up the returned delegate.
    /// Note that this string is typically *not* enough to regenerate the delegate, it is available for
    /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
    /// the returned delegate fails validation (indicative of a bug in Sigil) or
    /// behaves unexpectedly (indicative of a logic bug in the consumer code).
    /// </summary>
    public TDelegateType CreateDelegate(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        if (DynMethod == null)
        {
            throw new InvalidOperationException("Emit was not created to build a DynamicMethod, thus CreateDelegate cannot be called");
        }

        if (_createdDelegate != null)
        {
            instructions = null;
            return _createdDelegate;
        }

        _createdDelegate = (TDelegateType)(object)InnerCreateDelegate(typeof(TDelegateType), out instructions, optimizationOptions);

        return _createdDelegate;
    }

    /// <summary>
    /// Converts the CIL stream into a delegate.
    ///
    /// Validation that cannot be run until a method is finished is run, and various instructions
    /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
    ///
    /// Once this method is called the Emit may no longer be modified.
    /// </summary>
    public TDelegateType CreateDelegate(OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        return CreateDelegate( instructions: out _, optimizationOptions );
    }

    /// <summary>
    /// Writes the CIL stream out to the MethodBuilder used to create this Emit.
    ///
    /// Validation that cannot be run until a method is finished is run, and various instructions
    /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
    ///
    /// Once this method is called the Emit may no longer be modified.
    ///
    /// Returns a MethodBuilder, which can be used to define overrides or for further inspection.
    ///
    /// `instructions` will be set to a representation of the instructions making up the returned method.
    /// Note that this string is typically *not* enough to regenerate the method, it is available for
    /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
    /// the returned method fails validation (indicative of a bug in Sigil) or
    /// behaves unexpectedly (indicative of a logic bug in the consumer code).
    /// </summary>
    public MethodBuilder CreateMethod(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        if (MtdBuilder == null)
        {
            throw new InvalidOperationException("Emit was not created to build a method, thus CreateMethod cannot be called");
        }

        if (_methodBuilt)
        {
            instructions = null;
            return MtdBuilder;
        }

        Seal(optimizationOptions);

        _methodBuilt = true;

        var il = MtdBuilder.GetILGenerator();
        instructions = _il.UnBuffer(il);

        AutoNamer.Release(this);

        return MtdBuilder;
    }

    /// <summary>
    /// Writes the CIL stream out to the MethodBuilder used to create this Emit.
    ///
    /// Validation that cannot be run until a method is finished is run, and various instructions
    /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
    ///
    /// Once this method is called the Emit may no longer be modified.
    ///
    /// Returns a MethodBuilder, which can be used to define overrides or for further inspection.
    /// </summary>
    public MethodBuilder CreateMethod(OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        return CreateMethod( instructions: out _, optimizationOptions );
    }

    /// <summary>
    /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
    ///
    /// Validation that cannot be run until a method is finished is run, and various instructions
    /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
    ///
    /// Once this method is called the Emit may no longer be modified.
    ///
    /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
    ///
    /// `instructions` will be set to a representation of the instructions making up the returned constructor.
    /// Note that this string is typically *not* enough to regenerate the constructor, it is available for
    /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
    /// the returned constructor fails validation (indicative of a bug in Sigil) or
    /// behaves unexpectedly (indicative of a logic bug in the consumer code).
    /// </summary>
    public ConstructorBuilder CreateConstructor(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        if (ConstrBuilder == null || !IsBuildingConstructor)
        {
            throw new InvalidOperationException("Emit was not created to build a constructor, thus CreateConstructor cannot be called");
        }

        if (_constructorBuilt)
        {
            instructions = null;
            return ConstrBuilder;
        }

        _constructorBuilt = true;

        Seal(optimizationOptions);

        var il = ConstrBuilder.GetILGenerator();
        instructions = _il.UnBuffer(il);

        AutoNamer.Release(this);

        return ConstrBuilder;
    }

    /// <summary>
    /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
    ///
    /// Validation that cannot be run until a method is finished is run, and various instructions
    /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
    ///
    /// Once this method is called the Emit may no longer be modified.
    ///
    /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
    /// </summary>
    public ConstructorBuilder CreateConstructor(OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        return CreateConstructor( instructions: out _, optimizationOptions );
    }

    /// <summary>
    /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
    ///
    /// Validation that cannot be run until a method is finished is run, and various instructions
    /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
    ///
    /// Once this method is called the Emit may no longer be modified.
    ///
    /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
    ///
    /// `instructions` will be set to a representation of the instructions making up the returned constructor.
    /// Note that this string is typically *not* enough to regenerate the constructor, it is available for
    /// debugging purposes only.  Consumers may find it useful to log the instruction stream in case
    /// the returned constructor fails validation (indicative of a bug in Sigil) or
    /// behaves unexpectedly (indicative of a logic bug in the consumer code).
    /// </summary>
    public ConstructorBuilder CreateTypeInitializer(out string instructions, OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        if (ConstrBuilder == null || IsBuildingConstructor)
        {
            throw new InvalidOperationException("Emit was not created to build a type initializer, thus CreateTypeInitializer cannot be called");
        }

        if (_constructorBuilt)
        {
            instructions = null;
            return ConstrBuilder;
        }

        _constructorBuilt = true;

        Seal(optimizationOptions);

        var il = ConstrBuilder.GetILGenerator();
        instructions = _il.UnBuffer(il);

        AutoNamer.Release(this);

        return ConstrBuilder;
    }

    /// <summary>
    /// Writes the CIL stream out to the ConstructorBuilder used to create this Emit.
    ///
    /// Validation that cannot be run until a method is finished is run, and various instructions
    /// are re-written to choose "optimal" forms (Br may become Br_S, for example).
    ///
    /// Once this method is called the Emit may no longer be modified.
    ///
    /// Returns a ConstructorBuilder, which can be used to define overrides or for further inspection.
    /// </summary>
    public ConstructorBuilder CreateTypeInitializer(OptimizationOptions optimizationOptions = OptimizationOptions.All)
    {
        return CreateTypeInitializer( instructions: out _, optimizationOptions );
    }

    private static void ValidateNewParameters<TCheckDelegateType>()
    {
        var delType = typeof(TCheckDelegateType);

        var baseTypes = new HashSet<Type>();
        baseTypes.Add(delType);
#if NETSTANDARD
            var bType = delType.GetTypeInfo().BaseType;
#else
        var bType = delType.BaseType;
#endif
        while (bType != null)
        {
            baseTypes.Add(bType);
            bType = bType.BaseType;
        }

        if (!baseTypes.Contains(typeof(Delegate)))
        {
            throw new ArgumentException("DelegateType must be a delegate, found " + delType.FullName);
        }
    }
    // TODO: see https://github.com/dotnet/corefx/issues/4543 item 2
#if !NETSTANDARD
    internal static bool AllowsUnverifiableCode(Module m)
    {
        return Attribute.IsDefined(m, typeof(System.Security.UnverifiableCodeAttribute));
    }
#endif

    internal static bool AllowsUnverifiableCode(ModuleBuilder m)
    {
        var canaryMethod = new DynamicMethod("__Canary" + Guid.NewGuid(), typeof(void), [], m);
        var il = canaryMethod.GetILGenerator();
        il.Emit(OpCodes.Ldc_I4, 1024);
        il.Emit(OpCodes.Localloc);
        il.Emit(OpCodes.Pop);
        il.Emit(OpCodes.Ret);

        var d1 = (Action)canaryMethod.CreateDelegate(typeof(Action));

        try
        {
            d1();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates a new Emit, optionally using the provided name and module for the inner DynamicMethod.
    ///
    /// If name is not defined, a sane default is generated.
    ///
    /// If module is not defined, a module with the same trust as the executing assembly is used instead.
    ///
    /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
    /// of Sigil are reduced to "a nicer ILGenerator interface".
    ///
    /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
    /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
    /// </summary>
    public static Emit<TDelegateType> NewDynamicMethod(string name = null, ModuleBuilder module = null, bool doVerify = true, bool strictBranchVerification = false)
    {
        return DisassemblerDynamicMethod(name: name, module: module, doVerify: doVerify, strictBranchVerification: strictBranchVerification);
    }

    internal static Emit<TDelegateType> DisassemblerDynamicMethod(Type[] parameters = null, string name = null, ModuleBuilder module = null, bool doVerify = true, bool strictBranchVerification = false)
    {
        module = module ?? _module;

        name = name ?? AutoNamer.Next("_DynamicMethod");

        ValidateNewParameters<TDelegateType>();

        var delType = typeof(TDelegateType);

        var invoke = delType.GetMethod("Invoke");
        var returnType = invoke.ReturnType;
        var parameterTypes = parameters ?? invoke.GetParameters().Select(s => s.ParameterType).ToArray();

        var dynMethod = new DynamicMethod(name, returnType, parameterTypes, module, skipVisibility: true);

        var ret = new Emit<TDelegateType>( dynMethod.CallingConvention, returnType, parameterTypes, AllowsUnverifiableCode( module ), doVerify, strictBranchVerification )
        {
            DynMethod = dynMethod,
        };

        return ret;
    }

    /// <summary>
    /// Creates a new Emit, optionally using the provided name and owner for the inner DynamicMethod.
    ///
    /// If name is not defined, a sane default is generated.
    ///
    /// If owner is not defined, a module with the same trust as the executing assembly is used instead.
    ///
    /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
    /// of Sigil are reduced to "a nicer ILGenerator interface".
    ///
    /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
    /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
    /// </summary>
    public static Emit<TDelegateType> NewDynamicMethod(Type owner, string name = null, bool doVerify = true, bool strictBranchVerification = false)
    {
        if (owner == null)
        {
            return NewDynamicMethod(name: name, module: null);
        }

        name = name ?? AutoNamer.Next("_DynamicMethod");

        ValidateNewParameters<TDelegateType>();

        var delType = typeof(TDelegateType);

        var invoke = delType.GetMethod("Invoke");
        var returnType = invoke.ReturnType;
        var parameterTypes = (invoke.GetParameters()).Select(s => s.ParameterType).ToArray();

        var dynMethod = new DynamicMethod(name, returnType, parameterTypes, owner, skipVisibility: true);
        // TODO: see https://github.com/dotnet/corefx/issues/4543 item 2
#if NETSTANDARD
            const bool AllowUnverifiable = false;
#else
        // ReSharper disable once InconsistentNaming
        bool AllowUnverifiable = AllowsUnverifiableCode(owner.Module);
#endif
        var ret = new Emit<TDelegateType>( dynMethod.CallingConvention, returnType, parameterTypes, AllowUnverifiable, doVerify, strictBranchVerification )
        {
            DynMethod = dynMethod,
        };

        return ret;
    }

    internal static void CheckAttributesAndConventions(MethodAttributes attributes, CallingConventions callingConvention)
    {
        if ((attributes & ~(MethodAttributes.Abstract | MethodAttributes.Assembly | MethodAttributes.CheckAccessOnOverride | MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.FamORAssem | MethodAttributes.Final | MethodAttributes.HasSecurity | MethodAttributes.HideBySig | MethodAttributes.MemberAccessMask | MethodAttributes.NewSlot | MethodAttributes.PinvokeImpl | MethodAttributes.Private | MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.RequireSecObject | MethodAttributes.ReuseSlot | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName | MethodAttributes.Static | MethodAttributes.UnmanagedExport | MethodAttributes.Virtual | MethodAttributes.VtableLayoutMask)) != 0)
        {
            throw new ArgumentException("Unrecognized flag in attributes");
        }

        if ((callingConvention & ~(CallingConventions.Any | CallingConventions.ExplicitThis | CallingConventions.HasThis | CallingConventions.Standard | CallingConventions.VarArgs)) != 0)
        {
            throw new ArgumentException("Unrecognized flag in callingConvention");
        }

        if (HasFlag(attributes, MethodAttributes.Static) && HasFlag(callingConvention, CallingConventions.HasThis))
        {
            throw new ArgumentException("Static methods cannot have a this reference");
        }
    }

    private static bool HasFlag(MethodAttributes value, MethodAttributes flag)
    {
        return (value & flag) != 0;
    }

    private static bool HasFlag(CallingConventions value, CallingConventions flag)
    {
        return (value & flag) != 0;
    }

    /// <summary>
    /// Creates a new Emit, suitable for building a method on the given TypeBuilder.
    ///
    /// The DelegateType and MethodBuilder must agree on return types, parameter types, and parameter counts.
    ///
    /// If you intend to use unveriable code, you must set allowUnverifiableCode to true.
    ///
    /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
    /// of Sigil are reduced to "a nicer ILGenerator interface".
    ///
    /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
    /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
    /// </summary>
    public static Emit<TDelegateType> BuildMethod(TypeBuilder type, string name,
        MethodAttributes attributes, CallingConventions callingConvention, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        if (name == null)
        {
            throw new ArgumentNullException("name");
        }

        CheckAttributesAndConventions(attributes, callingConvention);

        ValidateNewParameters<TDelegateType>();

        var delType = typeof(TDelegateType);

        var invoke = delType.GetMethod("Invoke");
        var returnType = invoke.ReturnType;
        var parameterTypes = (invoke.GetParameters()).Select(s => s.ParameterType).ToArray();

        var methodBuilder = type.DefineMethod(name, attributes, callingConvention, returnType, parameterTypes);

        if (HasFlag(callingConvention, CallingConventions.HasThis))
        {
            // Shove `this` in front, can't require it because it doesn't exist yet!
            var pList = new List<Type>(parameterTypes);
            pList.Insert(0, type);

            parameterTypes = pList.ToArray();
        }

        var ret = new Emit<TDelegateType>( callingConvention, returnType, parameterTypes, allowUnverifiableCode, doVerify, strictBranchVerification )
        {
            MtdBuilder = methodBuilder,
        };

        return ret;
    }

    /// <summary>
    /// Convenience method for creating static methods.
    ///
    /// Equivalent to calling to BuildMethod, but with MethodAttributes.Static set and CallingConventions.Standard.
    /// </summary>
    public static Emit<TDelegateType> BuildStaticMethod(TypeBuilder type, string name, MethodAttributes attributes, bool allowUnverifiableCode = false, bool doVerify = true)
    {
        return BuildMethod(type, name, attributes | MethodAttributes.Static, CallingConventions.Standard, allowUnverifiableCode, doVerify);
    }

    /// <summary>
    /// Convenience method for creating instance methods.
    ///
    /// Equivalent to calling to BuildMethod, but with CallingConventions.HasThis.
    /// </summary>
    public static Emit<TDelegateType> BuildInstanceMethod(TypeBuilder type, string name, MethodAttributes attributes, bool allowUnverifiableCode = false, bool doVerify = true)
    {
        return BuildMethod(type, name, attributes, CallingConventions.HasThis, allowUnverifiableCode, doVerify);
    }

    /// <summary>
    /// Creates a new Emit, suitable for building a constructor on the given TypeBuilder.
    ///
    /// The DelegateType and TypeBuilder must agree on parameter types and parameter counts.
    ///
    /// If you intend to use unveriable code, you must set allowUnverifiableCode to true.
    ///
    /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
    /// of Sigil are reduced to "a nicer ILGenerator interface".
    ///
    /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
    /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
    /// </summary>
    public static Emit<TDelegateType> BuildConstructor(TypeBuilder type, MethodAttributes attributes, CallingConventions callingConvention = CallingConventions.HasThis, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        CheckAttributesAndConventions(attributes, callingConvention);

        if (!HasFlag(callingConvention, CallingConventions.HasThis))
        {
            throw new ArgumentException("Constructors always have a this reference");
        }

        ValidateNewParameters<TDelegateType>();

        var delType = typeof(TDelegateType);

        var invoke = delType.GetMethod("Invoke");
        var returnType = invoke.ReturnType;
        var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

        if (returnType != typeof(void))
        {
            throw new ArgumentException("DelegateType used must return void");
        }

        var constructorBuilder = type.DefineConstructor(attributes, callingConvention, parameterTypes);

        // Constructors always have a `this`
        var pList = new List<Type>(parameterTypes);
        pList.Insert(0, type);

        parameterTypes = pList.ToArray();

        var ret = new Emit<TDelegateType>( callingConvention, typeof( void ), parameterTypes, allowUnverifiableCode, doVerify, strictBranchVerification )
        {
            ConstrBuilder = constructorBuilder,
            IsBuildingConstructor = true,
        };

        return ret;
    }

    /// <summary>
    /// Creates a new Emit, suitable for building a type initializer on the given TypeBuilder.
    ///
    /// The DelegateType and TypeBuilder must agree on parameter types and parameter counts.
    ///
    /// If you intend to use unveriable code, you must set allowUnverifiableCode to true.
    ///
    /// If doVerify is false (default is true) Sigil will *not* throw an exception on invalid IL.  This is faster, but the benefits
    /// of Sigil are reduced to "a nicer ILGenerator interface".
    ///
    /// If strictBranchValidation is true (default is false) Sigil will enforce "Backward branch constraints" which are *technically* required
    /// for valid CIL, but in practice often ignored.  The most common case to set this option is if you are generating types to write to disk.
    /// </summary>
    public static Emit<TDelegateType> BuildTypeInitializer(TypeBuilder type, bool allowUnverifiableCode = false, bool doVerify = true, bool strictBranchVerification = false)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        ValidateNewParameters<TDelegateType>();

        var delType = typeof(TDelegateType);

        var invoke = delType.GetMethod("Invoke");
        var returnType = invoke.ReturnType;
        var parameters = invoke.GetParameters();

        if (returnType != typeof(void))
        {
            throw new ArgumentException("DelegateType used must return void");
        }

        if (parameters.Length > 0)
        {
            throw new ArgumentException("A type initializer can have no arguments.");
        }

        var constructorBuilder = type.DefineTypeInitializer();

        var ret = new Emit<TDelegateType>( CallingConventions.Standard, typeof( void ), [], allowUnverifiableCode, doVerify, strictBranchVerification )
        {
            ConstrBuilder = constructorBuilder,
        };

        return ret;
    }

    private void RemoveInstruction(int index)
    {
        _il.Remove(index);

        // We need to update our state to account for the new insertion
        foreach (var v in _branches.Where(w => w.Item3 >= index).ToList())
        {
            _branches.Remove(v);
            _branches.Add(Tuple.Create(v.Item1, v.Item2, v.Item3 - 1));
        }

        foreach (var kv in _marks.Where(w => w.Value >= index).ToList())
        {
            _marks[kv.Key] = kv.Value - 1;
        }

        for (var i = 0; i < _returns.Count; i++)
        {
            if (_returns[i] >= index)
            {
                _returns[i] = _returns[i] - 1;
            }
        }

        var needUpdateKeys = _branchPatches.Keys.Where(k => k >= index).ToList();

        foreach (var key in needUpdateKeys)
        {
            var cur = _branchPatches[key];
            _branchPatches.Remove(key);
            _branchPatches[key - 1] = cur;
        }

        foreach (var kv in _tryBlocks.Where(kv => kv.Value.Item1 >= index).ToList())
        {
            _tryBlocks[kv.Key] = Tuple.Create(kv.Value.Item1 - 1, kv.Value.Item2);
        }
        foreach (var kv in _tryBlocks.Where(kv => kv.Value.Item2 >= index).ToList())
        {
            _tryBlocks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 - 1);
        }

        foreach (var kv in _catchBlocks.Where(kv => kv.Value.Item1 >= index).ToList())
        {
            _catchBlocks[kv.Key] = Tuple.Create(kv.Value.Item1 - 1, kv.Value.Item2);
        }

        foreach (var kv in _catchBlocks.Where(kv => kv.Value.Item2 >= index).ToList())
        {
            _catchBlocks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 - 1);
        }

        foreach (var kv in _finallyBlocks.Where(kv => kv.Value.Item1 >= index).ToList())
        {
            _finallyBlocks[kv.Key] = Tuple.Create(kv.Value.Item1 - 1, kv.Value.Item2);
        }

        foreach (var kv in _finallyBlocks.Where(kv => kv.Value.Item2 >= index).ToList())
        {
            _finallyBlocks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 - 1);
        }

        foreach (var elem in _readonlyPatches.ToList())
        {
            if (elem.Item1 >= index)
            {
                var update = Tuple.Create(elem.Item1 - 1, elem.Item2);

                _readonlyPatches.Remove(elem);
                _readonlyPatches.Add(elem);
            }
        }
    }

    private void InsertInstruction(int index, OpCode instr)
    {
        _il.Insert(index, instr);

        // We need to update our state to account for the new insertion
        foreach (var v in _branches.Where(w => w.Item3 >= index).ToList())
        {
            _branches.Remove(v);
            _branches.Add(Tuple.Create(v.Item1, v.Item2, v.Item3 + 1));
        }

        foreach (var kv in _marks.Where(w => w.Value >= index).ToList())
        {
            _marks[kv.Key] = kv.Value + 1;
        }

        for (var i = 0; i < _returns.Count; i++)
        {
            if (_returns[i] >= index)
            {
                _returns[i] = _returns[i] + 1;
            }
        }

        var needUpdateKeys = _branchPatches.Keys.Where(k => k >= index).ToList();

        foreach (var key in needUpdateKeys)
        {
            var cur = _branchPatches[key];
            _branchPatches.Remove(key);
            _branchPatches[key + 1] = cur;
        }

        foreach (var kv in _tryBlocks.Where(kv => kv.Value.Item1 >= index).ToList())
        {
            _tryBlocks[kv.Key] = Tuple.Create(kv.Value.Item1 + 1, kv.Value.Item2);
        }

        foreach (var kv in _tryBlocks.Where(kv => kv.Value.Item2 >= index).ToList())
        {
            _tryBlocks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
        }

        foreach (var kv in _catchBlocks.Where(kv => kv.Value.Item1 >= index).ToList())
        {
            _catchBlocks[kv.Key] = Tuple.Create(kv.Value.Item1 + 1, kv.Value.Item2);
        }

        foreach (var kv in _catchBlocks.Where(kv => kv.Value.Item2 >= index).ToList())
        {
            _catchBlocks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
        }

        foreach (var kv in _finallyBlocks.Where(kv => kv.Value.Item1 >= index).ToList())
        {
            _finallyBlocks[kv.Key] = Tuple.Create(kv.Value.Item1 + 1, kv.Value.Item2);
        }

        foreach (var kv in _finallyBlocks.Where(kv => kv.Value.Item2 >= index).ToList())
        {
            _finallyBlocks[kv.Key] = Tuple.Create(kv.Value.Item1, kv.Value.Item2 + 1);
        }

        foreach (var elem in _readonlyPatches.ToList())
        {
            if (elem.Item1 >= index)
            {
                var update = Tuple.Create(elem.Item1 + 1, elem.Item2);

                _readonlyPatches.Remove(elem);
                _readonlyPatches.Add(elem);
            }
        }
    }

    private void UpdateStackAndInstrStream(OpCode? instr, TransitionWrapper transitions, bool firstParamIsThis = false)
    {
        if (_invalidated)
        {
            throw new InvalidOperationException("Cannot modify Emit after a delegate has been generated from it");
        }

        if (_mustMark)
        {
            throw new SigilVerificationException("Unreachable code detected", _il.Instructions(_allLocals));
        }

        var wrapped = new InstructionAndTransitions(instr, instr.HasValue ? (int?)_il.Index : null, transitions.Transitions);

        _typesProducedAtIndex[_il.Index] = transitions.Transitions.SelectMany(t => t.PushedToStack).ToList();

        var verifyRes = _currentVerifiers.Transition(wrapped);
        if (!verifyRes.Success)
        {
            throw new SigilVerificationException(transitions.MethodName, verifyRes, _il.Instructions(_allLocals));
        }

        if (_isVerifying)
        {
            MaxStackSize = Math.Max(verifyRes.StackSize, MaxStackSize);
        }
    }

    private void UpdateState(TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(null, transitions);
    }

    private void UpdateState(OpCode instr, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr);
    }

    private void UpdateState(OpCode instr, byte param, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param);
    }

    private void UpdateState(OpCode instr, short param, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param);
    }

    private void UpdateState(OpCode instr, int param, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param);
    }

    private void UpdateState(OpCode instr, uint param, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param);
    }

    private void UpdateState(OpCode instr, long param, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param);
    }

    private void UpdateState(OpCode instr, ulong param, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param);
    }

    private void UpdateState(OpCode instr, float param, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param);
    }

    private void UpdateState(OpCode instr, double param, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param);
    }

    private void UpdateState(OpCode instr, SigilLocal param, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param);
    }

    private void UpdateState(OpCode instr, SigilLabel param, TransitionWrapper transitions, out UpdateOpCodeDelegate update)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param, out update);
    }

    private void UpdateState(OpCode instr, SigilLabel[] param, TransitionWrapper transitions, out UpdateOpCodeDelegate update)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, param, out update);
    }

    private void UpdateState(OpCode instr, MethodInfo method, IEnumerable<Type> parameterTypes, TransitionWrapper transitions, bool firstParamIsThis = false, Type[] arglist = null)
    {
        UpdateStackAndInstrStream(instr, transitions, firstParamIsThis);

        if (arglist == null)
        {
            _il.Emit(instr, method, parameterTypes);
        }
        else
        {
            _il.EmitCall(instr, method, parameterTypes, arglist);
        }
    }

    private void UpdateState(OpCode instr, ConstructorInfo cons, IEnumerable<Type> parameterTypes, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions, firstParamIsThis: true);

        _il.Emit(instr, cons, parameterTypes);
    }

    private void UpdateState(OpCode instr, ConstructorInfo cons, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, cons);
    }

    private void UpdateState(OpCode instr, Type type, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, type);
    }

    private void UpdateState(OpCode instr, FieldInfo field, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, field);
    }

    private void UpdateState(OpCode instr, string str, TransitionWrapper transitions)
    {
        UpdateStackAndInstrStream(instr, transitions);

        _il.Emit(instr, str);
    }

    private void UpdateState(OpCode instr, CallingConventions callConventions, Type returnType, Type[] parameterTypes, TransitionWrapper transitions, Type[] arglist)
    {
        UpdateStackAndInstrStream(instr, transitions);

        if (arglist == null)
        {
            _il.Emit(instr, callConventions, returnType, parameterTypes);
        }
        else
        {
            _il.EmitCalli(callConventions, returnType, parameterTypes, arglist);
        }
    }

    private TransitionWrapper Wrap(IEnumerable<StackTransition> trans, string name)
    {
        return TransitionWrapper.Get(name, trans);
    }
}
