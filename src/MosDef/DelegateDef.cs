using ScrubJay.Reflection.Utilities;
using TextBuilder = ScrubJay.Text.TextBuilder;

namespace ScrubJay.Reflection.MosDef;



public class DelegateDefinition
{
    public static DelegateDefinition Create<D>()
        where D : Delegate
    {
        var invoke = DelegateHelper.InvokeMethod<D>();
        return new()
        {
            ReturnType = invoke.ReturnType,
            ParameterTypes = invoke.GetParameterTypes(),
        };
    }

    internal Type?   _returnType;
    internal Type[]? _parameterTypes;

    [AllowNull, NotNull]
    public Type ReturnType
    {
        get => _returnType ??= typeof(void);
        set => _returnType = value;
    }

    [AllowNull, NotNull]
    public Type[] ParameterTypes
    {
        get => _parameterTypes ??= [];
        set => _parameterTypes = value;
    }

    public override string ToString()
    {
        using var text = new TextBuilder();

        if (ReturnType.IsNullOrVoid())
        {
            text.Append("action<")
                .Delimit(", ", ParameterTypes, static (tb, pt) => tb.AppendType(pt))
                .Append('>');
        }
        else
        {
            text.Append("func<")
                .Enumerate(ParameterTypes, static (tb, pt) => tb.AppendType(pt).Append(", "))
                .AppendType(ReturnType)
                .Append('>');
        }
        return text.ToString();
    }
}

public class MethodDefinition : DelegateDefinition
{
    public static MethodDefinition Create<D>(string? name)
        where D : Delegate
    {
        var invoke = DelegateHelper.InvokeMethod<D>();
        return new()
        {
            Name = name,
            ReturnType = invoke.ReturnType,
            ParameterTypes = invoke.GetParameterTypes(),
        };
    }
    
    private string? _name;

    [AllowNull, NotNull]
    public string Name
    {
        get => _name ??= string.Empty;
        set => _name = value;
    }
}