namespace ScrubJay.Reflection.IL.Decompilation;
/// <summary>
/// An <see cref="ITokenProvider"/> that uses a <see cref="Module"/>
/// </summary>
[PublicAPI]
public sealed class ModuleTokenProvider : ITokenProvider
{
    private readonly Module _module;

    public ModuleTokenProvider(Module module)
    {
        _module = module;
    }
    
    public Result<FieldInfo> ResolveField(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments)
    {
        FieldInfo? field;
        try
        {
            field = _module.ResolveField(metadataToken, genericTypeArguments, genericMethodArguments);
        }
        catch (Exception ex)
        {
            return ex;
        }

        if (field is null)
        {
            return new InvalidOperationException($"Could not find Field #{metadataToken}");
        }
        
        return Ok(field);
    }

    public Result<MethodBase> ResolveMethod(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments)
    {
        MethodBase? method;
        try
        {
            method = _module.ResolveMethod(metadataToken, genericTypeArguments, genericMethodArguments);
        }
        catch (Exception ex)
        {
            return ex;
        }

        if (method is null)
        {
            return new InvalidOperationException($"Could not find Method #{metadataToken}");
        }
        
        return Ok(method);
    }

    public Result<Type> ResolveType(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments)
    {
        Type? type;
        try
        {
            type = _module.ResolveType(metadataToken, genericTypeArguments, genericMethodArguments);
        }
        catch (Exception ex)
        {
            return ex;
        }

        if (type is null)
        {
            return new InvalidOperationException($"Could not find Type #{metadataToken}");
        }
        
        return Ok(type);
    }

    public Result<MemberInfo> ResolveMember(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments)
    {
        MemberInfo? member;
        try
        {
            member = _module.ResolveMember(metadataToken, genericTypeArguments, genericMethodArguments);
        }
        catch (Exception ex)
        {
            return ex;
        }

        if (member is null)
        {
            return new InvalidOperationException($"Could not find Member #{metadataToken}");
        }
        
        return Ok(member);
    }


    public Result<byte[]> ResolveSignature(int metadataToken)
    {
        byte[]? signature;
        try
        {
            signature = _module.ResolveSignature(metadataToken);
        }
        catch (Exception ex)
        {
            return ex;
        }

        if (signature is null)
        {
            return new InvalidOperationException($"Could not find Signature #{metadataToken}");
        }

        return Ok(signature);
    }


    public Result<string> ResolveString(int metadataToken)
    {
        string? str;
        try
        {
            str = _module.ResolveString(metadataToken);
        }
        catch (Exception ex)
        {
            return ex;
        }
        
        if (str is null)
        {
            return new InvalidOperationException($"Could not find String #{metadataToken}");
        }

        return Ok(str);
    }
}