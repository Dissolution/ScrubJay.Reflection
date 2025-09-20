using ScrubJay.Reflection.Exceptions;

namespace ScrubJay.Reflection.Decompilation;

[PublicAPI]
public sealed class ModuleMemberMetadataResolver : IMemberMetadataResolver
{
    public Module Module { get; }
    public Type[]? DeclaredGenericTypes { get; }
    public Type[]? MethodGenericTypes { get; }

    public ModuleMemberMetadataResolver(Module module, Type[]? declaredGenericTypes, Type[]? methodGenericTypes)
    {
        Module = module;
        DeclaredGenericTypes = declaredGenericTypes;
        MethodGenericTypes = methodGenericTypes;
    }

    private ReflectionException GetEx<T>(MetadataToken metadataToken)
    {
        var message = TextBuilder.New
            .Render(Module)
            .Append("::Type")
            .RenderGenericTypes(DeclaredGenericTypes)
            .Append(".Method")
            .RenderGenericTypes(MethodGenericTypes)
            .NewLine()
            .Append($"Could not resolve metadata token {metadataToken:@} into a {typeof(T):@}")
            .ToStringAndDispose();
        return new ReflectionException(message);
    }
    
    public Result<FieldInfo> TryResolveField(MetadataToken metadataToken)
    {
        FieldInfo? field;
        try
        {
            field = Module.ResolveField(metadataToken, DeclaredGenericTypes, MethodGenericTypes);
        }
        catch (Exception ex)
        {
            return ex;
        }

        if (field is null)
            return GetEx<FieldInfo>(metadataToken);

        return Ok(field);
    }
    
    public Result<MethodBase> TryResolveMethod(MetadataToken metadataToken)
    {
        MethodBase? method;
        try
        {
            method = Module.ResolveMethod(metadataToken, DeclaredGenericTypes, MethodGenericTypes);
        }
        catch (Exception ex)
        {
            return ex;
        }

        if (method is null)
            return GetEx<MethodBase>(metadataToken);

        return Ok(method);
    }
    
    public Result<Type> TryResolveType(MetadataToken metadataToken)
    {
        Type? type;
        try
        {
            type = Module.ResolveType(metadataToken, DeclaredGenericTypes, MethodGenericTypes);
        }
        catch (Exception ex)
        {
            return ex;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (type is null)
        {
            return GetEx<Type>(metadataToken);
        }

        return Ok(type);
    }
    
    public Result<MemberInfo> TryResolveMember(MetadataToken metadataToken)
    {
        MemberInfo? member;
        try
        {
            member = Module.ResolveMember(metadataToken, DeclaredGenericTypes, MethodGenericTypes);
        }
        catch (Exception ex)
        {
            return ex;
        }

        if (member is null)
            return GetEx<MemberInfo>(metadataToken);

        return Ok(member);
    }

    public Result<string> TryResolveString(MetadataToken metadataToken)
    {
        return Result.Try(() => Module.ResolveString(metadataToken));
    }
    
    public Result<byte[]> TryResolveSignature(MetadataToken metadataToken)
    {
        return Result.Try(() => Module.ResolveSignature(metadataToken));
    }

}