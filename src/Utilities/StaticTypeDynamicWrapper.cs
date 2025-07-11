using System.Dynamic;
using System.Linq.Expressions;
using ScrubJay.Reflection.Searching;
using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection.Utilities;

[PublicAPI]
public abstract class DynamicWrapper
{
    public static dynamic WrapStaticType(Type staticType)
    {
        return StaticTypeDynamicWrapper.For(staticType);
    }
}

[PublicAPI]
public sealed class StaticTypeDynamicWrapper : DynamicObject, IDynamicMetaObjectProvider
{
    private static readonly ConcurrentTypeMap<StaticTypeDynamicWrapper> _cache = [];

    public static StaticTypeDynamicWrapper For(Type staticType)
    {
        MemberAssert.IsStatic(staticType);
        return _cache.GetOrAdd(staticType, static type => new(type));
    }


    private readonly Type _type;

    private StaticTypeDynamicWrapper(Type type)
    {
        Debug.Assert(type is not null);
        Debug.Assert(type!.IsStatic());
        _type = type!;
    }

    private (object?[] Args, Type[]? ArgTypes) Extract(object?[]? args)
    {
        if (args is null)
            return ([], []);
        int count = args.Length;
        Type[] argTypes = new Type[count];
        for (var i = 0; i < count; i++)
        {
            argTypes[i] = args[i]?.GetType() ?? typeof(object);
        }
        return (args, argTypes);
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
        return base.GetDynamicMemberNames();
    }

    public override DynamicMetaObject GetMetaObject(Expression parameter)
    {
        DynamicMetaObject dmo = base.GetMetaObject(parameter);
        if (dmo.GetType().Name != "MetaDynamic")
            Debugger.Break();
        return dmo;
    }

    
    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result)
    {
        Debugger.Break();
        return base.TryBinaryOperation(binder, arg, out result);
    }
    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
         Debugger.Break();
        return base.TryConvert(binder, out result);
    }
    public override bool TryCreateInstance(CreateInstanceBinder binder, object?[]? args, [NotNullWhen(true)] out object? result)
    {
        Debugger.Break();
        return base.TryCreateInstance(binder, args, out result);
    }
    public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
    {
        Debugger.Break();
        return base.TryDeleteIndex(binder, indexes);
    }
    public override bool TryDeleteMember(DeleteMemberBinder binder)
    {
        Debugger.Break();
        return base.TryDeleteMember(binder);
    }
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        Debugger.Break();
        return base.TryGetIndex(binder, indexes, out result);
    }
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        Debugger.Break();
        return base.TryGetMember(binder, out result);
    }
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        Debugger.Break();
        return base.TryInvoke(binder, args, out result);
    }

    private ParameterInfo[] ExtractParams(object?[]? args, CallInfo callInfo)
    {
        if (args is null)
        {
            Debug.Assert(callInfo.ArgumentCount == 0);
            return [];
        }
        
        int count = args.Length;
        Debug.Assert(callInfo.ArgumentCount == count);
        if (count == 0)
            return [];

        var argNames = callInfo.ArgumentNames;
        var parameters = new ParameterInfo[count];
        
        for (var i = 0; i < count; i++)
        {
            ParameterInfo param = new OverridableParameterInfo()
            {
                Position = i,
                Name = argNames[i],
                ParameterType = args[i]?.GetType() ?? typeof(object),
            };
            parameters[i] = param;
        }

        return parameters;
    }

    private (object?[] Args, Type[] ArgTypes) ExtractArgTypes(object?[]? arguments)
    {
        if (arguments is null) return ([], []);
        int count = arguments.Length;
        if (count == 0) return ([], []);

        object?[] args = new object?[count];
        Type[] argTypes = new Type[count];
        for (var i = 0; i < count; i++)
        {
            object? a = arguments[i];
            args[i] = a;
            argTypes[i] = a is null ? typeof(object) : a.GetType();
        }
        return (args, argTypes);
    }
    
//    private ObjectInvoke? CreateObjectInvoke(MemberSearchOptions key)
//    {
//        // Our common search flags
//        var flags = BindingFlags.Public | BindingFlags.NonPublic;
//        // Instance or static?
//        if (_target is null)
//            flags |= BindingFlags.Static;
//        else
//            flags |= BindingFlags.Instance;
//
//        // Zero args
//        if (key.ParameterTypes?.Length == 0)
//        {
//            // Might be field.get, property.get, or no-args method
//            FieldInfo? field = null;
//
//            // Check for Property
//            PropertyInfo? property = _targetType.GetProperty(key.Name!, flags);
//            if (property is not null)
//            {
//                // Do we have a getter to adapt?
//                var getter = property.GetGetter();
//                if (getter is not null)
//                {
//                    return RuntimeMethodAdapter.Adapt<ObjectInvoke>(getter);
//                }
//                // Backing field?
//                field = property.GetBackingField();
//            }
//
//            // Check for Field (if we didn't have one from Property, above)
//            if (field is null)
//            {
//                field = _targetType.GetField(key.Name!, flags);
//            }
//            if (field is not null)
//            {
//                throw new NotImplementedException();
//            }
//
//            // Fallthrough for Method check
//        }
//        // 1 arg
//        else if (key.ParameterTypes?.Length == 1)
//        {
//            // might be field.set, property.set
//            FieldInfo? field = null;
//
//            // Check for Property
//            PropertyInfo? property = _targetType.GetProperty(key.Name!, flags);
//            if (property is not null)
//            {
//                // Do we have a setter to adapt?
//                var setter = property.GetSetter();
//                if (setter is not null)
//                {
//                    return RuntimeMethodAdapter.Adapt<ObjectInvoke>(setter);
//                }
//                // Backing field?
//                field = property.GetBackingField();
//            }
//
//            // Check for Field (if we didn't have one from Property, above)
//            if (field is null)
//            {
//                field = _targetType.GetField(key.Name!, flags);
//            }
//            if (field is not null)
//            {
//                throw new NotImplementedException();
//            }
//
//            // Fallthrough for Method check
//        }
//
//        // Find a compatible method
//        MethodInfo? method;
//        var methods = _targetType.GetMethods(flags)
//            .SelectWhere((MethodInfo meth, out (MethodInfo Method, int Exactness) output) =>
//            {
//                output = default;
//                int exactness = 0;
//
//                // Has to have the right name
//                if (meth.Name != key.Name)
//                    return false;
//
//                // Has to have a compat return type
//                if (!((Arg)meth.ReturnType).CanLoadAs((Arg)key.ReturnType!, out int e))
//                    return false;
//                exactness += e;
//
//                // Has to have a compat parameter sig
//                if (!RuntimeMethodAdapter.CanAdaptTypes(key.ParameterTypes!, meth.GetParameterTypes(), out e))
//                    return false;
//                exactness += e;
//
//                // Matches!
//                output = (meth, exactness);
//                return true;
//            })
//            .OrderBy(tuple => tuple.Exactness)
//            .Select(tuple => tuple.Method)
//            .ToList();
//        method = methods.FirstOrDefault();
//        if (method is not null)
//            return RuntimeMethodAdapter.Adapt<ObjectInvoke>(methods[0]);
//
//        // Params Method?
//        method = methods
//            .Where(m => m.GetParameters().OneOrDefault()?.IsParams() == true)
//            .OneOrDefault();
//        if (method is not null)
//            return RuntimeMethodAdapter.Adapt<ObjectInvoke>(method);
//
//        // Nothing matches
//        Debug.WriteLine(Dump($"Nothing found on {_targetType} when searching for {key}"));
//        return null;
//    }
    
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? arguments, out object? result)
    {
        string memberName = binder.Name;
        bool ignoreCase = binder.IgnoreCase;
        var returnType = binder.ReturnType;

        if (returnType == typeof(Type))
            Debugger.Break();

        Mirror members = Reflect(_type)
            .Static
            .Named(memberName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        var (args, argTypes) = ExtractArgTypes(arguments);
        
        if (args.Length == 0)
        {
            // Field?
            if (members.OfType<FieldInfo>()
                .Where(f => f.FieldType.Implements(returnType))
                .TryGetOne()
                .IsOk(out var field))
            {
                // field getter
                object? value = field.GetValue(null);
                Debug.Assert(value.As(returnType).IsSome());
                result = value;
                return true;
            }
            
            // Property?
            if (members.OfType<PropertyInfo>()
                .Where(p => p.PropertyType.Implements(returnType))
                .Where(p => p.GetIndexParameters().IsNullOrEmpty())
                .TryGetOne()
                .IsOk(out var property))
            {
                // property getter
                object? value = property.GetValue(null);
                Debug.Assert(value.As(returnType).IsSome());
                result = value;
                return true;
            }
            
            // Method?
            if (members.OfType<MethodBase>()
                    .Where(m => m.GetParameters().Length == 0)
                    .Where(m => m.ReturnType().Implements(returnType))
                    .TryGetOne()
                    .IsOk(out var method))
            {
                object? value = method.Invoke(null, null);
                Debug.Assert(value.As(returnType).IsSome());
                result = value;
                return true;
            }
            
            // no match
            Debugger.Break();
            result = null;
            return false;
        }
        else if (argTypes.Length == 1)
        {
            // Field?
            if (members.OfType<FieldInfo>()
                .Where(f => returnType.Implements(f.FieldType))
                .TryGetOne()
                .IsOk(out var field))
            {
                // field setter
                field.SetValue(null, args[0]);
                
                Debugger.Break();
                result = DBNull.Value;
                return true;
            }
            
            // Property?
            if (members.OfType<PropertyInfo>()
                .Where(p => returnType.Implements(p.PropertyType))
                .TryGetOne()
                .IsOk(out var property))
            {
                // property setter
                property.SetValue(null, args[0]);
                
                Debugger.Break();
                result = DBNull.Value;
                return true;
            }
         
           

        }

        // Method?
        var methods = members
            .MethodBases()
            .Only(argTypes, static (method, aTypes) => method.Parameters().CanAcceptA((aTypes)));

        if (returnType == typeof(void))
        {
            // We can invoke _any_ matching method
            if (methods.TryGetFirst().IsOk(out var method))
            {
                object? value = method.Invoke(null, args);
                Debug.Assert(value.As(returnType).IsSome());
                result = value;
                return true;
            }
        }
        else if (returnType == typeof(object))
        {
            // We have no idea what we want?
            if (methods.Returning(typeof(void))
                .TryGetFirst().IsOk(out var method))
            {
                object? value = method.Invoke(null, args);
                Debug.Assert(value is null);
                result = this; // chain self
                return true;
            }
        }
        

        var invoked = base.TryInvokeMember(binder, args, out var baseResult);
        Debugger.Break();
        
        result = default;
        return false;
    }
    
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
    {
        Debugger.Break();
        return base.TrySetIndex(binder, indexes, value);
    }
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        Debugger.Break();
        return base.TrySetMember(binder, value);
    }
    public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result)
    {
        Debugger.Break();
        return base.TryUnaryOperation(binder, out result);
    }
    
    public override bool Equals(object? obj)
    {
        bool equals = base.Equals(obj);
        Debugger.Break();
        return equals;
    }
    
    public override int GetHashCode()
    {
        return Hasher.Hash(_type);
    }
    
    public override string ToString()
    {
        return $"dynamic_static({_type.Render()})";
    }
}