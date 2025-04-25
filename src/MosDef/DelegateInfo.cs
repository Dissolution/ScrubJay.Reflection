using ScrubJay.Reflection.Utilities;
using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection.MosDef;

[PublicAPI]
public sealed record class DelegateInfo
{
    public static DelegateInfo Create<D>()
        where D : Delegate
        => Create(typeof(D));
    
    public static DelegateInfo Create(Type delegateType)
    {
        MemberAssert.IsDelegateType(delegateType);
        var invokeMethod = delegateType.InvokeMethod().SomeOrThrow();
        DelegateInfo info = new()
        {
            Attributes = Attribute.GetCustomAttributes(delegateType),
            Name = delegateType.Name,
            GenericTypes = delegateType.GetGenericArguments(),
            ReturnParameter = invokeMethod.ReturnParameter,
            Parameters = invokeMethod.GetParameters(),
        };
        return info;
    }

    public static DelegateInfo Create(MethodInfo method)
    {
        Throw.IfNull(method);
        DelegateInfo info = new()
        {
            Attributes = Attribute.GetCustomAttributes(method),
            Name = method.Name,
            GenericTypes = method.GetGenericArguments(),
            ReturnParameter = method.ReturnParameter,
            Parameters = method.GetParameters(),
        };
        return info;
    }
    
    public static DelegateInfo Create(Delegate del)
    {
        Throw.IfNull(del);
        var invoke = ReflectOn(del)
            .Methods().Named("Invoke")
            .AsList();
        Debugger.Break();
        throw new NotImplementedException();
    }

    private Type[]? _parameterTypes = null;
    
    public Attribute[] Attributes { get; init; } = [];

    public string? Name { get; init; } = null;

    public Type[] GenericTypes { get; init; } = [];
    
    public required ParameterInfo ReturnParameter { get; init; }
    
    public required ParameterInfo[] Parameters { get; init; }

    public Type[] ParameterTypes => _parameterTypes ??= Parameters.ConvertAll(static p => p.ParameterType);
    
    private DelegateInfo()
    {
        
    }

    public override string ToString() => TextBuilder.New
        .IfNotEmpty(Attributes,
            static (tb, attrs) => tb.Append('[').Delimit(", ", attrs, static (t, a) => t.AppendAttribute(a)).Append("] "))
        .AppendParameter(ReturnParameter)
        .Append(' ')
        .AppendNameAndGenericTypes(Name, GenericTypes)
        .AppendParameters(Parameters)
        .ToStringAndDispose();


}