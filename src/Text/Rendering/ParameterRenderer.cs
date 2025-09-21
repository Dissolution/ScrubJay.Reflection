// using ScrubJay.Text.Rendering;
//
// namespace ScrubJay.Reflection.Text.Rendering;
//
// [PublicAPI]
// public sealed class ParameterRenderer : Renderer<ParameterInfo>
// {
//     public override TextBuilder RenderTo(TextBuilder builder, ParameterInfo? parameter)
//     {
//         if (parameter is null)
//             return builder;
//
//         (TRK paramRef, Type paramType) = parameter;
//         (string? prefix, string? postfix) = Nullability.Get(parameter).GetPrefixPostfix();
//         
//         return builder
//             .IfNotEmpty(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
//             .Append(paramRef.AsString())
//             .Render(paramType)
//             .Append(postfix)
//             .IfNotEmpty(parameter.Name, static (tb, name) => tb.Append(' ').Append(name))
//             .If(parameter.Default,
//                 static (tb, defaultValue) => tb.Append(" = ").Render(defaultValue));
//     }
// }