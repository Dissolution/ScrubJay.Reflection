using MA = System.Reflection.MethodAttributes;

namespace ScrubJay.Reflection.Signatures;

public record class MethodSig : MemberSig
{
    public static implicit operator MethodSig(MethodInfo method) => new(method);

    public ParameterSig Return { get; init; } = new()
    {
        Index = -1,
    };

    public Parameters Parameters { get; init; } = [];

    public bool Final { get; set; }
    public bool Virtual { get; set; }
    public bool Abstract { get; set; }



    public MA MethodAttributes
    {
        get
        {
            MA attr = default;
            var visFlag = Visibility switch
            {
                Visibility.Private => MA.Private,
                Visibility.Protected => MA.Family,
                Visibility.Internal => MA.Assembly,
                Visibility.Public => MA.Public,
                Visibility.NonPublic => MA.Private | MA.Family | MA.Assembly | MA.FamORAssem,
                _ => default,
            };
            attr.AddFlag(visFlag);
            if (Access.HasFlags(Access.Static))
                attr.AddFlag(MA.Static);
            if (Final)
                attr.AddFlag(MA.Final);
            if (Virtual)
                attr.AddFlag(MA.Virtual);
            if (Abstract)
                attr.AddFlag(MA.Abstract);
            return attr;
        }
    }


    public MethodSig() { }

    public MethodSig(MethodInfo methodInfo)
    {
        this.Attributes = new(Attribute.GetCustomAttributes(methodInfo));
        this.Name = methodInfo.Name;
        this.Return = methodInfo.ReturnParameter;
        this.Parameters = methodInfo.GetParameters();
    }
}