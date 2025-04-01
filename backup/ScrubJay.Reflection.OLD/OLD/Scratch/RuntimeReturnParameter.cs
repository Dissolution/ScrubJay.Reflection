using System.Reflection;

namespace ScrubJay.Reflection.OLD.OLD.Scratch;

public sealed record class RuntimeReturnParameter : RuntimeParameter
{
    public override ParameterAttributes ParameterAttributes
    {
        get
        {
            var attr = base.ParameterAttributes;
            attr.AddFlag(ParameterAttributes.Retval);
            return attr;
        }
    }

    public RuntimeReturnParameter()
    {
        this.Position = 0;
        this.Name = "return";
    }
}