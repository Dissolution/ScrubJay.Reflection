using ScrubJay.Reflection.Extensions;
using ScrubJay.Reflection.Text;

namespace ScrubJay.Reflection.Naming;

/// <summary>
/// Helper Utility for getting the names of <see cref="ParameterInfo"/> and <see cref="MemberInfo"/>
/// </summary>
[PublicAPI]
public static class MemberNames
{
   

    public static string NameOf(this ParameterInfo parameter)
    {
        return TextBuilder.New.Invoke(tb => tb.AppendParameter(parameter)).ToStringAndDispose();
    }

    public static string NameOf(this MemberInfo member)
    {
        return TextBuilder.New.Invoke(tb => tb.AppendMember(member)).ToStringAndDispose();
    }
}
