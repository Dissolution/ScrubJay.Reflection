//using ScrubJay.Reflection.Runtime.Naming;
//
//namespace ScrubJay.Reflection.Runtime;
//
//public static class TypeBuilderExtensions
//{
//    public static DynamicMethod CreateDynamicMethod(this TypeBuilder typeBuilder, MethodSignature signature)
//    {
//        // Fix name
//        string name = NameHelper.CreateMemberName(MemberTypes.Method, signature.NameFrom);
//        return new DynamicMethod(
//            name: name,
//            attributes: MethodAttributes.Public | MethodAttributes.Static, // only valid value
//            callingConvention: CallingConventions.Standard, // only valid value
//            returnType: signature.ReturnSignature,
//            parameterTypes: signature.ParameterSignatures,
//            owner: typeBuilder,
//            skipVisibility: true);
//    }
//}