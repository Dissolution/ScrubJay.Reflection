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
        
        
        
        var resolver = typeof(DynamicMethod).GetField("m_resolver", BF.Instance | BF.NonPublic).GetValue(dynamicMethod);
        if (resolver == null) throw new ArgumentException("The dynamic method's IL has not been finalized.");

        _tokenResolver = (TokenResolver)resolver.GetType().GetMethod("ResolveToken", BF.Instance | BF.NonPublic).CreateDelegate(typeof(TokenResolver), resolver);
        _stringResolver = (StringResolver)resolver.GetType().GetMethod("GetStringLiteral", BF.Instance | BF.NonPublic).CreateDelegate(typeof(StringResolver), resolver);
        _signatureResolver = (SignatureResolver)resolver.GetType().GetMethod("ResolveSignature", BF.Instance | BF.NonPublic).CreateDelegate(typeof(SignatureResolver), resolver);

        _getTypeFromHandleUnsafe = (GetTypeFromHandleUnsafe)typeof(Type).GetMethod("GetTypeFromHandleUnsafe", BF.Static | BF.NonPublic, null, new[] { typeof(IntPtr) }, null).CreateDelegate(typeof(GetTypeFromHandleUnsafe), null);
        var runtimeType = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeType");

        var runtimeMethodHandleInternal = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeMethodHandleInternal");
        _getMethodBase = runtimeType.GetMethod("GetMethodBase", BF.Static | BF.NonPublic, null, new[] { runtimeType, runtimeMethodHandleInternal }, null);
        _runtimeMethodHandleInternalCtor = runtimeMethodHandleInternal.GetConstructor(BF.Instance | BF.NonPublic, null, new[] { typeof(IntPtr) }, null);

        var runtimeFieldInfoStub = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeFieldInfoStub");
        _runtimeFieldHandleStubCtor = runtimeFieldInfoStub.GetConstructor(BF.Instance | BF.Public, null, new[] { typeof(IntPtr), typeof(object) }, null);
        _getFieldInfo = runtimeType.GetMethod("GetFieldInfo", BF.Static | BF.NonPublic, null, new[] { runtimeType, typeof(RuntimeTypeHandle).Assembly.GetType("System.IRuntimeFieldInfo") }, null);
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