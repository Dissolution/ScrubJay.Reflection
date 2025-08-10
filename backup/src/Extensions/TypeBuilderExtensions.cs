namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class TypeBuilderExtensions
{
    public static DynamicMethod CreateDynamicMethod(this TypeBuilder typeBuilder, DelegateInfo signature)
    {
        return new DynamicMethod(
            name: CodeHelper.GetValidMemberName(MemberTypes.Method, signature.Name),
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            callingConvention: CallingConventions.Standard,
            returnType: signature.ReturnType,
            parameterTypes: signature.ParameterTypes,
            owner: typeBuilder,
            skipVisibility: true);
    }
}