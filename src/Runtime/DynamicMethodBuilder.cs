namespace ScrubJay.Reflection.Runtime;

public sealed record class RuntimeReturnParameter : RuntimeParameter
{
    public override ParameterAttributes ParameterAttributes
    {
        get
        {
            var attr = base.ParameterAttributes;
            attr.AddFlag(ParameterAttributes.Retval);
            return attr;
        }
    }

    public RuntimeReturnParameter()
    {
        this.Position = 0;
        this.Name = "return";
    }
}

public record class RuntimeParameter
{
    public int Position { get; set; } = -1; // invalid
    public string? Name { get; set; } = null;
    public Type? ValueType { get; set; } = null;
    public ReferenceType ReferenceType { get; set; } = ReferenceType.Default;
    public bool IsOptional { get; set; } = false;
    public Option<object?> DefaultValue { get; set; } = None();

    public virtual ParameterAttributes ParameterAttributes
    {
        get
        {
            ParameterAttributes attr = default;
            if (IsOptional)
                attr.AddFlag(ParameterAttributes.Optional);
            if (DefaultValue.IsSome())
                attr.AddFlag(ParameterAttributes.HasDefault);
            if (ReferenceType.HasFlags(ReferenceType.In))
                attr.AddFlag(ParameterAttributes.In);
            if (ReferenceType.HasFlags(ReferenceType.Out))
                attr.AddFlag(ParameterAttributes.Out);
            return attr;
        }
    }
}

public class RuntimeParameterBuilder
{
    internal RuntimeParameter _runtimeParameter = new();

    public RuntimeParameterBuilder Position(int position)
    {
        _runtimeParameter.Position = position;
        return this;
    }
    
    public RuntimeParameterBuilder Name(string name)
    {
        _runtimeParameter.Name = name;
        return this;
    }

    public RuntimeParameterBuilder ValueType(Type type)
    {
        _runtimeParameter.ReferenceType = type.ReferenceType(out var cleanType);
        _runtimeParameter.ValueType = cleanType;
        return this;
    }

    public RuntimeParameterBuilder ReferenceType(ReferenceType referenceType)
    {
        _runtimeParameter.ReferenceType = referenceType;
        return this;
    }

    public RuntimeParameterBuilder IsOptional(bool isOptional)
    {
        _runtimeParameter.IsOptional = isOptional;
        return this;
    }

    public RuntimeParameterBuilder HasDefaultValue(object? defaultValue)
    {
        _runtimeParameter.DefaultValue = Some(defaultValue);
        return this;
    }

    public RuntimeParameter GetParameter() => _runtimeParameter;
}

public class DynamicMethodBuilder
{
    private Module? _module;
    private string? _name;
    
    private RuntimeParameter? _returnParameter;
    
    private RuntimeParameter[]? _argParameters;
    
    public DynamicMethodBuilder Name(string name)
    {
        Validate.ThrowIfEmpty(name);
        _name = name;
        return this;
    }

    public DynamicMethodBuilder ReturnType(Type? type)
    {
        _returnParameter = new RuntimeParameterBuilder()
            .ValueType(type ?? typeof(void))
            .GetParameter();
        return this;
    }

    public DynamicMethodBuilder ReturnType(Action<RuntimeParameterBuilder> buildReturn)
    {
        var builder = new RuntimeParameterBuilder();
        buildReturn(builder);
        _returnParameter = builder._runtimeParameter;
        return this;
    }

    public DynamicMethodBuilder ParameterTypes(params Type[] types)
    {
        _argParameters = types;
        return this;
    }

    public DynamicMethodBuilder Module(Module module)
    {
        _module = module;
        return this;
    }

    public DynamicMethod GetDynamicMethod()
    {
        var dm = new DynamicMethod(
            name: _name ?? Guid.NewGuid().ToString(),
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            callingConvention: CallingConventions.Standard,
            returnType: _returnType ?? typeof(void),
            parameterTypes: _argParameters ?? [],
            m: _module ?? RuntimeBuilder.ModuleBuilder,
            skipVisibility: true);

        dm.DefineParameter(0, 

        return dm;
    }
    
    
    
}