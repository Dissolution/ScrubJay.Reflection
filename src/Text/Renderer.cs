// using ScrubJay.Debugging;
// using ScrubJay.Reflection.Collections;
//
// namespace ScrubJay.Reflection.Text;
//
// public static class Renderer
// {
//     private static readonly DelegateMap _cachedRenderers = [];
//
//     public delegate void RenderValueTo<B, in T>(TextBuilderBase<B> builder, T? value)
//         where B : TextBuilderBase<B>;
//
//     public static void Map<B, T>(RenderValueTo<B, T> renderer)
//         where B : TextBuilderBase<B>
//     {
//         _cachedRenderers.TryAdd(renderer);
//     }
//
//     public static B Render<B, T>(this B builder, T[]? array)
//         where B : TextBuilderBase<B>
//     {
//         return builder
//             .IfNotNull(array,
//                 static (tb, arr) => tb.Append('[').Delimit(", ", arr, static (t, a) => t.Render(a)).Append(']'),
//                 static tb => tb.Append("null"));
//     }
//
//     public static B Render<B, T>(this B builder, T? value)
//         where B : TextBuilderBase<B>
//     {
//         if (value is IRenderable)
//         {
//             ((IRenderable)value).RenderTo(builder);
//             return builder;
//         }
//
//         if (_cachedRenderers.TryGet<RenderValueTo<B, T>>(out var renderer))
//         {
//             renderer(builder, value);
//             return builder;
//         }
//
//         switch (value)
//         {
//             case null:
//                 return builder.Append("`null`");
//             case DBNull:
//                 return builder.Append(nameof(DBNull));
//             case bool b:
//                 return builder.AppendIf(b, bool.TrueString, bool.FalseString);
//             case byte u8:
//                 return builder.Append("(byte)").Append(u8);
//             case sbyte i8:
//                 return builder.Append("(sbyte)").Append(i8);
//             case short i16:
//                 return builder.Append("(short)").Append(i16);
//             case ushort u16:
//                 return builder.Append("(ushort)").Append(u16);
//             case int i32:
//                 return builder.Append(i32);
//             case uint u32:
//                 return builder.Append(u32).Append('U');
//             case long i64:
//                 return builder.Append(i64).Append('L');
//             case ulong u64:
//                 return builder.Append(u64).Append("UL");
//             case float f32:
//                 return builder.Append(f32, "N1").Append('f');
//             case double f64:
//                 return builder.Append(f64, "N1").Append('d');
//             case decimal dec:
//                 return builder.Append(dec, "N1").Append('m');
//             case TimeSpan ts:
//                 return builder.Append(ts, "g");
//             case DateTime dt:
//                 return builder.Append(dt, "yyyy-MM-dd HH:mm:ss");
//             case Guid guid:
//                 return builder.Append(guid.ToUpperDigitsString());
//             case char ch:
//                 return builder.Append('\'').Append(ch).Append('\'');
//             case string str:
//                 return builder.Append('"').Append(str).Append('"');
//             case ParameterInfo parameter:
//                 return builder.AppendParameter(parameter);
//             case MemberInfo member:
//                 return builder.AppendMember(member);
//             case LocalVariableInfo local:
//             {
//                 return builder
//                     .Append($"[{local.LocalIndex}] ")
//                     .AppendIf(local.IsPinned, "fixed ")
//                     .AppendType(local.LocalType);
//             }
//             case Exception ex:
//                 return builder.Append(ex.Dump());
// #if !NETSTANDARD2_0
//             case ITuple tuple:
//             {
//                 return builder.Append('(')
//                     .Delimit(", ", Enumerable.Range(0, tuple.Length),
//                         (tb, i) => tb.Render(tuple[i]))
//                     .Append(')');
//             }
// #endif
//             case Enum e:
//             {
//                 return builder.Append(e.ToString());
//             }
//             case object?[] objectArray:
//             {
//                 return builder.Append("object[")
//                     .Delimit(", ", objectArray, static (tb,obj) => tb.Render(obj))
//                     .Append(']');
//             }
//             default:
//             {
//                 string? str = value.ToString();
//                 return builder.Append(str);
//             }
//         }
//     }
// }