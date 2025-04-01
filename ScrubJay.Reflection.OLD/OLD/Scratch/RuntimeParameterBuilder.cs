namespace ScrubJay.Reflection.OLD.OLD.Scratch;

public class RuntimeParameterBuilder : FluentBuilder<RuntimeParameterBuilder>
{
    internal RuntimeParameter _runtimeParameter = new();

    public RuntimeParameterBuilder Position(int position)
    {
        _runtimeParameter.Position = position;
        return _builder;
    }
    
    public RuntimeParameterBuilder Name(string name)
    {
        _runtimeParameter.Name = name;
        return _builder;
    }

    public RuntimeParameterBuilder ValueType(Type? type)
    {
        if (type is null)
        {
            _runtimeParameter.ReferenceType = Reflection.OLD.ReferenceType.Default;
            _runtimeParameter.ValueType = typeof(void);
        }
        else
        {
            _runtimeParameter.ReferenceType = type.ReferenceType(out var cleanType);
            _runtimeParameter.ValueType = cleanType;
        }

        return _builder;
    }

    public RuntimeParameterBuilder ReferenceType(ReferenceType referenceType)
    {
        _runtimeParameter.ReferenceType = referenceType;
        return _builder;
    }

    public RuntimeParameterBuilder IsOptional(bool isOptional)
    {
        _runtimeParameter.IsOptional = isOptional;
        return _builder;
    }

    public RuntimeParameterBuilder HasDefaultValue(object? defaultValue)
    {
        _runtimeParameter.DefaultValue = Some(defaultValue);
        return _builder;
    }

    public RuntimeParameter GetParameter() => _runtimeParameter;
}