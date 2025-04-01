using ScrubJay.Fluent;

namespace ScrubJay.Sigil.Emission.Fluent;

public class DynamicParameter : ParameterInfo
{

}

public class BufferedEmitter<TSelf> : FluentBuilder<TSelf>
    where TSelf : BufferedEmitter<TSelf>
{
    private readonly DynamicMethod _dynamicMethod;

    public CallingConventions CallingConventions => _dynamicMethod.CallingConvention;
    public ParameterInfo ReturnParameter => _dynamicMethod.ReturnParameter;
    public ParameterInfo[] Parameters => _dynamicMethod.GetParameters();
}

public class EmitterValidator
{
    public bool AllowUnverifiable { get; set; }
    public bool DoVerify { get; set; }
    public bool StrictBranchVerification { get; set; }
}
