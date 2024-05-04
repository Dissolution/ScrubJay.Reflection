﻿using Jay.Reflection.Building.Emission;

namespace Jay.Reflection.Building;

public class RuntimeDelegateBuilder
{
    protected readonly DynamicMethod _dynamicMethod;
    protected readonly DelegateInfo _delegateSig;
    private IFluentILEmitter? _emitter;

    public DynamicMethod DynamicMethod => _dynamicMethod;

    public string Name => _dynamicMethod.Name;
    public MethodAttributes Attributes => _dynamicMethod.Attributes;
    public CallingConventions CallingConventions => _dynamicMethod.CallingConvention;

    public Type ReturnType => _delegateSig.ReturnType;

    public ParameterInfo[] Parameters => _delegateSig.Parameters;

    public ParameterInfo? FirstParameter => Parameters.Length > 0 ? Parameters[0] : null;
    
    public Type[] ParameterTypes => _delegateSig.ParameterTypes;

    public int ParameterCount => _delegateSig.ParameterCount;

    public DelegateInfo Signature => _delegateSig;

    //public ILGenerator IlGenerator => _dynamicMethod.GetILGenerator();

    public IFluentILEmitter Emitter => _emitter ??= new FluentILGenerator(_dynamicMethod.GetILGenerator());

    internal RuntimeDelegateBuilder(DynamicMethod dynamicMethod, DelegateInfo concreteSignature)
    {
        _dynamicMethod = dynamicMethod;
        _delegateSig = concreteSignature;
    }

    public Delegate CreateDelegate()
    {
        return _dynamicMethod.CreateDelegate(_delegateSig.GetDelegateType());
    }
}