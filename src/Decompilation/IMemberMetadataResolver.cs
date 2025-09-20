namespace ScrubJay.Reflection.Decompilation;

[PublicAPI]
public interface IMemberMetadataResolver
{
    Result<FieldInfo> TryResolveField(MetadataToken metadataToken);
    Result<MethodBase> TryResolveMethod(MetadataToken metadataToken);
    Result<Type> TryResolveType(MetadataToken metadataToken);
    Result<MemberInfo> TryResolveMember(MetadataToken metadataToken);
    Result<string> TryResolveString(MetadataToken metadataToken);
    Result<byte[]> TryResolveSignature(MetadataToken metadataToken);
}