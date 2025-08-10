//using System.Dynamic;
//
//using Jay.Dumping.Extensions;
//using Jay.Reflection.Building.Adaption;
//using Jay.Reflection.Comparison;
//using Jay.Reflection.Extensions;
//using Jay.Reflection.Searching;
//using Jay.Utilities;
//
//namespace Jay.Reflection;
//
//internal delegate object? ObjectInvoke([Instance] object? instance, params object?[] args);
//
//
//public sealed class DynamicReflection : DynamicObject
//{
//    public static dynamic Of(object obj) => new DynamicReflection(obj);
//    public static dynamic Of(Type staticType) => new DynamicReflection(staticType);
//
//
//    private object? _target;
//    private readonly Type _targetType;
//    private readonly IEqualityComparer _equalityComparer;
//    private readonly IComparer _comparer;
//    private readonly Dictionary<MemberSearchOptions, ObjectInvoke?> _delegateCache;
//
//    private DynamicReflection(object? target, Type targetType)
//    {
//        if ((target == null) != (targetType.IsStatic()))
//            throw new ArgumentException();
//        _target = target;
//        _targetType = targetType;
//        _equalityComparer = DefaultComparers.Instance;
//        _comparer = DefaultComparers.Instance;
//        _delegateCache = new();
//    }
//    private DynamicReflection(object obj)
//        : this(obj, obj.GetType())
//    {
//    }
//    private DynamicReflection(Type staticType)
//        : this(null, staticType)
//    {
//    }
//
//    private bool TryGetObjectInvoke(MemberSearchOptions key, [NotNullWhen(true)] out ObjectInvoke? objectInvoke)
//    {
//        objectInvoke = _delegateCache.GetOrAdd(key, CreateObjectInvoke);
//        return objectInvoke is not null;
//    }
//
//    private bool TryGetObjectInvoke(MemberSearchOptions key, Func<MemberSearchOptions, MethodBase?> findMethod,
//        [NotNullWhen(true)] out ObjectInvoke? objectInvoke)
//    {
//        objectInvoke = _delegateCache.GetOrAdd(key,
//            k =>
//            {
//                var method = findMethod(k);
//                if (method is null) return null;
//                if (RuntimeMethodAdapter.TryAdapt<ObjectInvoke>(method, out var oi))
//                    return oi;
//                return null;
//            });
//        return objectInvoke is not null;
//    }
//
//
//  
//
//    private static (object?[] Objects, Type[] ArgTypes) FixArgs(object?[]? args)
//    {
//        if (args is null)
//            return (Array.Empty<object?>(), Array.Empty<Type>());
//        var argTypes = new Type[args.Length];
//        for (var i = 0; i < args.Length; i++)
//        {
//            argTypes[i] = args[i]?.GetType() ?? typeof(object);
//        }
//        return (args, argTypes);
//    }
//
//
//
//    public override IEnumerable<string> GetDynamicMemberNames()
//    {
//        var memberNames = base.GetDynamicMemberNames().ToList();
//        Debugger.Break();
//        return memberNames;
//    }
//
//    public override DynamicMetaObject GetMetaObject(Expression parameter)
//    {
//        DynamicMetaObject meta = base.GetMetaObject(parameter);
//        Debug.WriteLine($"{meta:I} GetDynamicMetaObject({parameter})");
//        return meta;
//    }
//
//    #region Operators
//    private MemberSearchOptions GetKey(ExpressionType expressionType, Type returnType, params Type[] argTypes)
//    {
//        switch (expressionType)
//        {
//            case ExpressionType.Add:
//                break;
//            case ExpressionType.AddChecked:
//                break;
//            case ExpressionType.And:
//                {
//                    return new("op_BitwiseAnd", returnType, argTypes.Single());
//                }
//            case ExpressionType.AndAlso:
//                break;
//            case ExpressionType.ArrayLength:
//                break;
//            case ExpressionType.ArrayIndex:
//                break;
//            case ExpressionType.Call:
//                break;
//            case ExpressionType.Coalesce:
//                break;
//            case ExpressionType.Conditional:
//                break;
//            case ExpressionType.Constant:
//                break;
//            case ExpressionType.Convert:
//                break;
//            case ExpressionType.ConvertChecked:
//                break;
//            case ExpressionType.Divide:
//                break;
//            case ExpressionType.Equal:
//                {
//                    return new("op_Equality", typeof(bool), Validate.LengthIs(argTypes, 2));
//                }
//            case ExpressionType.ExclusiveOr:
//                break;
//            case ExpressionType.GreaterThan:
//                break;
//            case ExpressionType.GreaterThanOrEqual:
//                break;
//            case ExpressionType.Invoke:
//                break;
//            case ExpressionType.Lambda:
//                break;
//            case ExpressionType.LeftShift:
//                break;
//            case ExpressionType.LessThan:
//                break;
//            case ExpressionType.LessThanOrEqual:
//                break;
//            case ExpressionType.ListInit:
//                break;
//            case ExpressionType.MemberAccess:
//                break;
//            case ExpressionType.MemberInit:
//                break;
//            case ExpressionType.Modulo:
//                break;
//            case ExpressionType.Multiply:
//                break;
//            case ExpressionType.MultiplyChecked:
//                break;
//            case ExpressionType.Negate:
//                break;
//            case ExpressionType.UnaryPlus:
//                break;
//            case ExpressionType.NegateChecked:
//                break;
//            case ExpressionType.New:
//                break;
//            case ExpressionType.NewArrayInit:
//                break;
//            case ExpressionType.NewArrayBounds:
//                break;
//            case ExpressionType.Not:
//                break;
//            case ExpressionType.NotEqual:
//                break;
//            case ExpressionType.Or:
//                break;
//            case ExpressionType.OrElse:
//                break;
//            case ExpressionType.Parameter:
//                break;
//            case ExpressionType.Power:
//                break;
//            case ExpressionType.Quote:
//                break;
//            case ExpressionType.RightShift:
//                break;
//            case ExpressionType.Subtract:
//                break;
//            case ExpressionType.SubtractChecked:
//                break;
//            case ExpressionType.TypeAs:
//                break;
//            case ExpressionType.TypeIs:
//                break;
//            case ExpressionType.Assign:
//                break;
//            case ExpressionType.Block:
//                break;
//            case ExpressionType.DebugInfo:
//                break;
//            case ExpressionType.Decrement:
//                break;
//            case ExpressionType.Dynamic:
//                break;
//            case ExpressionType.Default:
//                break;
//            case ExpressionType.Extension:
//                break;
//            case ExpressionType.Goto:
//                break;
//            case ExpressionType.Increment:
//                break;
//            case ExpressionType.Index:
//                break;
//            case ExpressionType.Label:
//                break;
//            case ExpressionType.RuntimeVariables:
//                break;
//            case ExpressionType.Loop:
//                break;
//            case ExpressionType.Switch:
//                break;
//            case ExpressionType.Throw:
//                break;
//            case ExpressionType.Try:
//                break;
//            case ExpressionType.Unbox:
//                break;
//            case ExpressionType.AddAssign:
//                break;
//            case ExpressionType.AndAssign:
//                break;
//            case ExpressionType.DivideAssign:
//                break;
//            case ExpressionType.ExclusiveOrAssign:
//                break;
//            case ExpressionType.LeftShiftAssign:
//                break;
//            case ExpressionType.ModuloAssign:
//                break;
//            case ExpressionType.MultiplyAssign:
//                break;
//            case ExpressionType.OrAssign:
//                break;
//            case ExpressionType.PowerAssign:
//                break;
//            case ExpressionType.RightShiftAssign:
//                break;
//            case ExpressionType.SubtractAssign:
//                break;
//            case ExpressionType.AddAssignChecked:
//                break;
//            case ExpressionType.MultiplyAssignChecked:
//                break;
//            case ExpressionType.SubtractAssignChecked:
//                break;
//            case ExpressionType.PreIncrementAssign:
//                break;
//            case ExpressionType.PreDecrementAssign:
//                break;
//            case ExpressionType.PostIncrementAssign:
//                break;
//            case ExpressionType.PostDecrementAssign:
//                break;
//            case ExpressionType.TypeEqual:
//                break;
//            case ExpressionType.OnesComplement:
//                break;
//            case ExpressionType.IsTrue:
//                break;
//            case ExpressionType.IsFalse:
//                {
//                    return new("op_False", typeof(bool), argTypes.Single());
//                }
//            default:
//                throw new ArgumentOutOfRangeException(nameof(expressionType), expressionType, null);
//        }
//
//        var methods = _targetType.GetMethods(Reflect.Flags.Static);
//        var d = Dump((expressionType, returnType, argTypes, methods));
//        Debugger.Break();
//        throw new NotImplementedException();
//    }
//
//    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result)
//    {
//        var key = GetKey(binder.Operation, binder.ReturnType, _targetType, arg.GetType());
//        BindingFlags flags = Reflect.Flags.Static;
//
//        Debugger.Break();
//
//        if (TryGetObjectInvoke(key,
//                k => _targetType.GetMethod(k.Name!, flags),
//                out var objectInvoke))
//        {
//            result = objectInvoke(_target, arg);
//            return true;
//        }
//
//
//        var eqMethods = _targetType.GetMethods(Reflect.Flags.All)
//            .Where(method => method.Name.Contains("eq", StringComparison.OrdinalIgnoreCase))
//            .Dump();
//        var opType = binder.Operation.GetType();
//        Debugger.Break();
//        throw new NotImplementedException();
//
//
//    }
//
//    public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result)
//    {
//        var key = GetKey(binder.Operation, binder.ReturnType, _targetType);
//        var flags = Reflect.Flags.Static;
//
//        Debugger.Break();
//
//        if (TryGetObjectInvoke(key,
//                k => _targetType.GetMethod(k.Name!, flags),
//                out var objectInvoke))
//        {
//            result = objectInvoke(_target);
//            return true;
//        }
//
//        result = default;
//        return false;
//    }
//
//    public override bool TryConvert(ConvertBinder binder, out object? result)
//    {
//        bool b = base.TryConvert(binder, out result);
//        Debugger.Break();
//        return b;
//    }
//    #endregion
//
//    #region Indexers
//    /* Indexers
//     * Normally, only instances may have indexers (a silly C# thing),
//     * but we know the names of the methods the compiler outputs for instance indexers
//     * and a great adapter, so we can fake indexers on static classes.
//     *
//     * Indexer Setter:
//     * void set_Item(key, value?);
//     * void set_Item(key1,..keyN, value?);
//     *
//     * Indexer Getter:
//     * value? get_Item(key);
//     * value? get_Item(key1,..keyN);
//     */
//
//    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
//    {
//        var (objects, argTypes) = FixArgs(indexes);
//        MemberSearchOptions key = new("get_Item", binder.ReturnType, argTypes);
//        if (TryGetObjectInvoke(key, out var objectInvoke))
//        {
//            result = objectInvoke(_target, objects);
//            return true;
//        }
//
//        result = default;
//        return false;
//    }
//    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
//    {
//        // To use ObjectInvoke, we're going to need to combine indexes + value into a single array
//        var objects = new object?[indexes.Length + 1];
//        indexes.CopyTo(objects.AsSpan(0));
//        objects[^1] = value;
//        var argTypes = objects.ToTypeArray();
//
//        // do not use binder.ReturnType, it will be object and we definitely want void
//        MemberSearchOptions key = new("set_Item", typeof(void), argTypes);
//        if (TryGetObjectInvoke(key, out var objectInvoke))
//        {
//            objectInvoke(_target, objects);
//            return true;
//        }
//
//        return false;
//    }
//
//    public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
//    {
//        bool b = base.TryDeleteIndex(binder, indexes);
//        Debugger.Break();
//        return b;
//    }
//    #endregion
//
//    #region Member Interaction
//    public override bool TryGetMember(GetMemberBinder binder, out object? result)
//    {
//        MemberSearchOptions key = new(binder.Name, binder.ReturnType);
//        if (TryGetObjectInvoke(key, out var objectInvoke))
//        {
//            result = objectInvoke(_target);
//            return true;
//        }
//        result = default;
//        return false;
//    }
//
//    public override bool TrySetMember(SetMemberBinder binder, object? value)
//    {
//        MemberSearchOptions key = new(binder.Name, typeof(void), typeof(object));
//        if (TryGetObjectInvoke(key, out var objectInvoke))
//        {
//            objectInvoke(_target, value);
//            return true;
//        }
//        return false;
//    }
//
//    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
//    {
//        var (objects, argTypes) = FixArgs(args);
//        MemberSearchOptions key = new(binder.Name, binder.ReturnType, argTypes);
//        if (TryGetObjectInvoke(key, out var objectInvoke))
//        {
//            result = objectInvoke(_target, objects);
//            return true;
//        }
//        result = default;
//        return false;
//    }
//
//    public override bool TryDeleteMember(DeleteMemberBinder binder)
//    {
//        bool b = base.TryDeleteMember(binder);
//        Debugger.Break();
//        return b;
//    }
//    #endregion
//
//    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
//    {
//        var (objects, argTypes) = FixArgs(args);
//        MemberSearchOptions key = new("Invoke", binder.ReturnType, argTypes);
//        if (TryGetObjectInvoke(key, out var objectInvoke))
//        {
//            result = objectInvoke(_target, objects);
//            return true;
//        }
//        result = default;
//        return false;
//    }
//
//    public override bool TryCreateInstance(CreateInstanceBinder binder, object?[]? args, [NotNullWhen(true)] out object? result)
//    {
//        bool b = base.TryCreateInstance(binder, args, out result);
//        Debugger.Break();
//        return b;
//    }
//
//    public int CompareTo(object? other)
//    {
//        return _comparer.Compare(_target, other);
//    }
//
//    public override bool Equals(object? obj)
//    {
//        return _equalityComparer.Equals(_target, obj);
//    }
//
//    public override int GetHashCode()
//    {
//        return Hasher.Combine(_target, _targetType);
//    }
//
//    public override string ToString()
//    {
//        return Dump($"dynamic<{_targetType}>");
//    }
//}