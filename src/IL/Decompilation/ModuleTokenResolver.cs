namespace ScrubJay.Reflection.IL.Decompilation;

public sealed class ModuleTokenResolver : ITokenResolver
{
    private readonly Module _module;

    public ModuleTokenResolver(Module module)
    {
        _module = module;
    }

    public FieldInfo? ResolveField(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments) =>
        _module.ResolveField(metadataToken, genericTypeArguments, genericMethodArguments);

    public MethodBase? ResolveMethod(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments)
    {
        try
        {
            return _module.ResolveMethod(metadataToken, genericTypeArguments, genericMethodArguments);
        }
        catch (Exception ex)
        {
            return null;
        }
       
    }

    public Type ResolveType(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments) =>
        _module.ResolveType(metadataToken, genericTypeArguments, genericMethodArguments);

    public MemberInfo? ResolveMember(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments) =>
        _module.ResolveMember(metadataToken, genericTypeArguments, genericMethodArguments);


    public byte[] ResolveSignature(int metadataToken) =>
        _module.ResolveSignature(metadataToken);

    
    public string ResolveString(int metadataToken) =>
        _module.ResolveString(metadataToken);
}