using ScrubJay.Text.Comparison;

namespace ScrubJay.Reflection.IL.Decompilation;

public static class DecompileHelper
{
    public static ITokenProvider GetTokenResolver(MethodBase method)
    {
        if (method is DynamicMethod dm)
            return new DynamicMethodITokenProvider(dm);
        return new ModuleTokenProvider(method.Module);
    }

    public static IList<LocalVariableInfo> GetLocals(MethodBase method)
    {
        if (method is DynamicMethod dm)
        {
            throw new NotImplementedException();
        }

        var body = method.GetMethodBody();
        if (body is null)
        {
            return [];
        }

        return body.LocalVariables;
    }

    public static byte[] GetILBytes(MethodBase method)
    {
        if (method is DynamicMethod dm)
            return GetILBytes(dm);

        var body = method.GetMethodBody()
            .ThrowIfNull("Cannot get Method Body");
        var ilBytes = body.GetILAsByteArray()
            .ThrowIfNull("Cannot get IL Bytes");
        return ilBytes;
    }

    public static byte[] GetILBytes(DynamicMethod dynamicMethod)
    {
        // Find the DynamicMethod's Resolver field
        var resolverField = Reflect<DynamicMethod>()
            .Fields()
            .Instance
            .NonPublic
            .Named("_resolver", new StringMatch(StringComparison.OrdinalIgnoreCase)
            {
                EndsWith = true,
            })
            .OneOrThrow("DynamicMethod does not contain a '_resolver' field");

        object? resolver = resolverField.GetValue(dynamicMethod);
        if (resolver == null)
        {
            Debugger.Break();
            throw new ArgumentException("The dynamic method's IL has not been finalized.");
        }

        var codeField = ReflectOn(resolver)
            .Instance
            .NonPublic
            .Fields<byte[]>()
            .Named("_code", new StringMatch(StringComparison.OrdinalIgnoreCase)
            {
                EndsWith = true,
            })
            .OneOrThrow("DynamicMethod's Resolver does not contain a '_code' field");

        object? code = codeField.GetValue(resolver);
        if (code is null)
        {
            Debugger.Break();
            throw new InvalidOperationException();
        }

        byte[] bytes = code.ThrowIfNot<byte[]>();
        return bytes;
    }
}