
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP
#endif

using ScrubJay.Text.Comparison;

namespace ScrubJay.Reflection.IL.Decompilation;

public sealed class DynamicMethodITokenProvider : ITokenProvider
{
    private delegate void TokenResolver(int token, out IntPtr typeHandle, out IntPtr methodHandle, out IntPtr fieldHandle);
    private delegate string StringResolver(int token);
    private delegate byte[] SignatureResolver(int token, int fromMethod);
    private delegate Type GetTypeFromHandleUnsafe(IntPtr handle);

    private readonly TokenResolver           _tokenResolver;
    private readonly StringResolver          _stringResolver;
    private readonly SignatureResolver       _signatureResolver;
    private readonly GetTypeFromHandleUnsafe _getTypeFromHandleUnsafe;
    private readonly MethodInfo              _getMethodBase;
    private readonly ConstructorInfo         _runtimeMethodHandleInternalCtor;
    private readonly ConstructorInfo         _runtimeFieldHandleStubCtor;
    private readonly MethodInfo              _getFieldInfo;

    public DynamicMethodITokenProvider(DynamicMethod dynamicMethod)
    {
        var resolverField = Shard<DynamicMethod>()
            .Fields().Instance().NonPublic()
            .Named("resolver", new StringMatch(StringComparison.Ordinal) { Contains = true })
            .OneOrThrow();
        var resolver = resolverField.GetValue(dynamicMethod);
        if (resolver is null) 
            throw new ArgumentException("The DynamicMethod's IL has not been finalized", nameof(dynamicMethod));

        var resolveTokenMethod = ShardOn(resolver)
            .Methods()
            .Instance()
            .NonPublic()
            .Named("ResolveToken")
            .OneOrThrow();
        _tokenResolver = resolveTokenMethod.CreateDelegate<TokenResolver>(resolver);

        var getStringLiteralMethod = ShardOn(resolver)
            .Methods().Instance().NonPublic()
            .Named("GetStringLiteral")
            .OneOrThrow();
        _stringResolver = getStringLiteralMethod.CreateDelegate<StringResolver>(resolver);

        var resolveSignatureMethod = ShardOn(resolver)
            .Methods().Instance().NonPublic()
            .Named("ResolveSignature")
            .OneOrThrow();
        _signatureResolver = resolveSignatureMethod.CreateDelegate<SignatureResolver>(resolver);

        var getTypeFromHandleUnsafeMethod = Shard<Type>()
            .Methods().Static()
            .Named("GetTypeFromHandleUnsafe")
            .WithParameters<IntPtr>()
            .OneOrThrow();
        _getTypeFromHandleUnsafe = getTypeFromHandleUnsafeMethod.CreateDelegate<GetTypeFromHandleUnsafe>();

        var runtimeType = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeType")
            .ThrowIfNull();

        var runtimeMethodHandleInternal = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeMethodHandleInternal")
            .ThrowIfNull();

        _getMethodBase = Shard(runtimeType)
            .Methods().Static()
            .Named("GetMethodBase")
            .WithParameters(runtimeType, runtimeMethodHandleInternal)
            .OneOrThrow();
        
        _runtimeMethodHandleInternalCtor = Shard(runtimeMethodHandleInternal)
            .Instance().NonPublic()
            .Constructors()
            .WithParameters<IntPtr>()
            .OneOrThrow();

        var runtimeFieldInfoStub = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeFieldInfoStub").ThrowIfNull();
        
        _runtimeFieldHandleStubCtor = Shard(runtimeFieldInfoStub)
            .Instance().Public()
            .Constructors()
            //.Parameters<IntPtr, object>()
            .OneOrThrow();

        _getFieldInfo = Shard(runtimeType)
            .Static().Methods()
            .Named("GetFieldInfo")
            .WithParameters(runtimeType, typeof(RuntimeTypeHandle).Assembly.GetType("System.IRuntimeFieldInfo").ThrowIfNull())
            .OneOrThrow();
    }

    public Result<Type> ResolveType(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments)
    {
        IntPtr typeHandle, methodHandle, fieldHandle;
        _tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

        return _getTypeFromHandleUnsafe.Invoke(typeHandle);
    }

    public Result<MethodBase> ResolveMethod(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments)
    {
        IntPtr typeHandle, methodHandle, fieldHandle;
        _tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

        return (MethodBase)_getMethodBase.Invoke(null, new[]
        {
            typeHandle == IntPtr.Zero ? null : _getTypeFromHandleUnsafe.Invoke(typeHandle),
            _runtimeMethodHandleInternalCtor.Invoke(new object[] { methodHandle })
        });
    }

    public Result<FieldInfo> ResolveField(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments)
    {
        IntPtr typeHandle, methodHandle, fieldHandle;
        _tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

        return (FieldInfo)_getFieldInfo.Invoke(null, new[]
        {
            typeHandle == IntPtr.Zero ? null : _getTypeFromHandleUnsafe.Invoke(typeHandle),
            _runtimeFieldHandleStubCtor.Invoke(new object[] { fieldHandle, null })
        });
    }

    public Result<MemberInfo> ResolveMember(int metadataToken, Type[]? genericTypeArguments, Type[]? genericMethodArguments)
    {
        IntPtr typeHandle, methodHandle, fieldHandle;
        _tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

        if (methodHandle != IntPtr.Zero)
        {
            return (MethodBase)_getMethodBase.Invoke(null, new[]
            {
                typeHandle == IntPtr.Zero ? null : _getTypeFromHandleUnsafe.Invoke(typeHandle),
                _runtimeMethodHandleInternalCtor.Invoke(new object[] { methodHandle })
            });
        }

        if (fieldHandle != IntPtr.Zero)
        {
            return (FieldInfo)_getFieldInfo.Invoke(null, new[]
            {
                typeHandle == IntPtr.Zero ? null : _getTypeFromHandleUnsafe.Invoke(typeHandle),
                _runtimeFieldHandleStubCtor.Invoke(new object[] { fieldHandle, null })
            });
        }

        if (typeHandle != IntPtr.Zero)
        {
            return _getTypeFromHandleUnsafe.Invoke(typeHandle);
        }

        throw new NotImplementedException("DynamicMethods are not able to reference members by token other than types, methods and fields.");
    }

    public Result<byte[]> ResolveSignature(int metadataToken)
    {
        return _signatureResolver.Invoke(metadataToken, 0);
    }

    public Result<string> ResolveString(int metadataToken)
    {
        return _stringResolver.Invoke(metadataToken);
    }
}