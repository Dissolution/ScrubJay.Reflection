namespace ScrubJay.Reflection.IL.Decompilation;

public interface ITokenProvider
{
    Result<FieldInfo> ResolveField(int metadataToken, Type[]? genericTypeArguments = null, Type[]? genericMethodArguments = null);
    Result<MethodBase> ResolveMethod(int metadataToken, Type[]? genericTypeArguments = null, Type[]? genericMethodArguments = null);
    Result<Type> ResolveType(int metadataToken, Type[]? genericTypeArguments = null, Type[]? genericMethodArguments = null);
    Result<MemberInfo> ResolveMember(int metadataToken, Type[]? genericTypeArguments = null, Type[]? genericMethodArguments = null);
    Result<byte[]> ResolveSignature(int metadataToken);
    Result<string> ResolveString(int metadataToken);
}

