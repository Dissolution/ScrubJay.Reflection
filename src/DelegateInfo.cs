using ScrubJay.Reflection.Validation;

namespace ScrubJay.Reflection;

/// <summary>
/// Information about a <see cref="Delegate"/>
/// </summary>
[PublicAPI]
public sealed record class DelegateInfo : IRenderable
{
    public static DelegateInfo New<D>(string? name = null)
        where D : Delegate
        => new(typeof(D), name);

    public static DelegateInfo New(Type delegateType, string? name = null)
        => new(delegateType, name);

    public static DelegateInfo New(MethodBase method, string? name = null)
        => new(method, name);


    private Type[]? _parameterTypes = null;
    private Type? _delegateType = null;


    public Attribute[] Attributes { get; init; } = [];

    public string? Name { get; init; } = null;

    public Type[] GenericTypes { get; init; } = [];

    public required ParameterInfo ReturnParameter { get; init; }

    public Type ReturnType => ReturnParameter.ParameterType;

    public required ParameterInfo[] Parameters { get; init; }

    public Type[] ParameterTypes => _parameterTypes ??= Parameters.ConvertAll(static p => p.ParameterType);

    public int ParameterCount => Parameters.Length;

    public Type DelegateType
    {
        get => _delegateType ??= DelegateHelper.CreateDelegateType(ParameterTypes, ReturnType);
        init => _delegateType = value;
    }

    public DelegateInfo()
    {
    }

    [SetsRequiredMembers]
    public DelegateInfo(Type delegateType, string? name = null)
    {
        MemberAssert.IsDelegateType(delegateType);
        
        var invokeMethod = delegateType.InvokeMethod().SomeOrThrow();
        Attributes = Attribute.GetCustomAttributes(delegateType);
        Name = name ?? delegateType.Name;
        GenericTypes = delegateType.GetGenericArguments();
        ReturnParameter = invokeMethod.ReturnParameter;
        Parameters = invokeMethod.GetParameters();
        DelegateType = delegateType;
    }

    [SetsRequiredMembers]
    public DelegateInfo(MethodBase method, string? name = null)
    {
        Throw.IfNull(method);

        Attributes = Attribute.GetCustomAttributes(method);
        Name = name ?? method.Name;
        GenericTypes = method.GetGenericArguments();
        ReturnParameter = method.ReturnParameter();
        Parameters = method.GetParameters();
    }

    
    
    public void RenderTo(TextBuilder builder)
    {
        builder
            .IfNotEmpty(Attributes,
                static (tb, attrs) => tb
                    .Append('[')
                    .EnumerateAndDelimit(attrs, static (t, a) => t.Render(a), ", ")
                    .Append(']'))
            .Render(ReturnParameter)
            .Append(' ')
            .NameGenericsParameters(Name, GenericTypes, Parameters);
    }

    public override string ToString() => TextBuilder.Build(RenderTo);
}