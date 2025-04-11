using ScrubJay.Text.Comparison;

namespace ScrubJay.Reflection.IL.Decompilation;

public static class ReflectionExtensions
{
    public static ITokenResolver GetTokenResolver(MethodBase method)
    {
        if (method is DynamicMethod dm)
            return new DynamicMethodTokenResolver(dm);
        return new ModuleTokenResolver(method.Module);
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
            var sig = TextBuilder.Build(tb => tb.AppendMethod(method));
            Debugger.Break();
            throw new InvalidOperationException("No Body");
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
            .Fields
            .Instance
            .NonPublic
            .Named("_resolver", new StringMatch.Ordinal()
            {
                EndsWith = true,
                IgnoreCase = true,
            })
            .OneOrThrow("DynamicMethod does not contain a '_resolver' field");

        object? resolver = resolverField.GetValue(dynamicMethod);
        if (resolver == null)
        {
            Debugger.Break();
            throw new ArgumentException("The dynamic method's IL has not been finalized.");
        }

        var codeField = ReflectOn(resolver)
            .Fields
            .Instance
            .NonPublic
            .Named("_code", new StringMatch.Ordinal()
            {
                EndsWith = true,
                IgnoreCase = true,
            })
            .Returning<byte[]>()
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