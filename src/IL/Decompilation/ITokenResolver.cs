namespace ScrubJay.Reflection.IL.Decompilation;

public interface ITokenResolver
{
    FieldInfo ResolveField(int metadataToken, Type[]? genericTypeArguments = null, Type[]? genericMethodArguments = null);
    MethodBase ResolveMethod(int metadataToken, Type[]? genericTypeArguments = null, Type[]? genericMethodArguments = null);
    Type ResolveType(int metadataToken, Type[]? genericTypeArguments = null, Type[]? genericMethodArguments = null);
    MemberInfo ResolveMember(int metadataToken, Type[]? genericTypeArguments = null, Type[]? genericMethodArguments = null);

    byte[] ResolveSignature(int metadataToken);

    string ResolveString(int metadataToken);
}