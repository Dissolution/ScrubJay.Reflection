global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace ScrubJay.Reflection.Signatures;

public record class DynamicMethodInfo
{
    public static DynamicMethodInfo Create<TD>(string? name = null)
        where TD : Delegate
    {
        return new(DelegateHelper.GetInvokeMethod<TD>())
        {
            Name = name,
        };
    }

    public string? Name { get; init; } = null;
    public required DynamicParameterInfo Return { get; init; }
    public required DynamicParameterInfo[] Parameters { get; init; }

    public DynamicMethodInfo() { }

    [SetsRequiredMembers]
    public DynamicMethodInfo(Type? returnType, params Type[]? parameterTypes)
    {
        Return = new DynamicParameterInfo()
        {
            Position = 0,
            Attributes = ParameterAttributes.Retval,
            Type = returnType ?? typeof(void),
        };
        if (parameterTypes is not null)
        {
            int len = parameterTypes.Length;
            Parameters = new DynamicParameterInfo[len];
            for (int i = 0; i < len; i++)
            {
                Parameters[i] = new DynamicParameterInfo()
                {
                    Position = (i + 1),
                    Type = parameterTypes[i],
                };
            }
        }
        else
        {
            Parameters = [];
        }
    }

    [SetsRequiredMembers]
    public DynamicMethodInfo(MethodBase method)
    {
        Throw.IfNull(method);
        Name = method.Name;
        if (method is MethodInfo methodInfo)
        {
            this.Return = new(methodInfo.ReturnParameter);
        }
        else if (method is ConstructorInfo constructorInfo)
        {
            this.Return = new(0, constructorInfo.ReturnType())
            {
                Attributes = ParameterAttributes.Retval,
            };
        }
        else
        {
            throw new UnreachableException();
        }

        var parameters = method.GetParameters();
        Parameters = new DynamicParameterInfo[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            Parameters[i] = new(parameters[i]);
        }
    }
}

public record class DynamicParameterInfo
{
    public required int Position { get; init; }
    public string? Name { get; init; } = null;
    public ParameterAttributes Attributes { get; init; } = ParameterAttributes.None;
    public required Type Type { get; init; }

    public DynamicParameterInfo() { }

    [SetsRequiredMembers]
    public DynamicParameterInfo(int position, Type type)
    {
        this.Position = position;
        this.Type = type;
    }

    [SetsRequiredMembers]
    public DynamicParameterInfo(ParameterInfo parameter)
    {
        Throw.IfNull(parameter);
        this.Position = parameter.Position;
        this.Name = parameter.Name;
        this.Type = parameter.ParameterType;
        this.Attributes = parameter.Attributes;
    }
}
