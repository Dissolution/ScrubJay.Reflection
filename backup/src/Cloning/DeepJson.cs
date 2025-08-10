// using ScrubJay.Reflection.Collections;
//
// namespace ScrubJay.Reflection.Cloning;
//
// public static class DeepJson
// {
//     private const string INDENT = "    ";
//     private static readonly DelegateMap _cache = new();
//
//     private static bool IsNumber(object obj)
//     {
//         return obj is
//             byte or sbyte or short or ushort or int or uint or long or ulong or nint or nuint or IntPtr or UIntPtr;
//     }
//
//     private static bool IsFloatingPoint(object obj)
//     {
//         return obj is
//             float or double or decimal;
//     }
//
//
//     private static IndentTextBuilder JsonAppendArray(this IndentTextBuilder builder, Array array)
//     {
//         var elementType = array.GetType().GetElementType().ThrowIfNull();
//
//         Action<IndentTextBuilder> delimit;
//         if (elementType.IsPrimitive)
//         {
//             delimit = static tb => tb.Append(", ");
//         }
//         else
//         {
//             delimit = static tb => tb.Append(',').NewLine();
//         }
//         
//         builder.Append('[')
//             .EnumerateAndDelimit(array.OfType<object?>(),
//                 static (tb, item) => tb.JsonAppendNameValue(null, item),
//                 delimit)
//             .Append(']');
//         
//         return builder;
//     }
//     
//     
//     private static IndentTextBuilder JsonAppendComplex(this IndentTextBuilder builder, object value)
//     {
//         builder.Append('{').AddIndent(INDENT).NewLine();
//
//         Type valueType = value.GetType();
//         
//         var fields = Reflect(valueType)
//             .Instance.Fields()
//             .AsList();
//
//         if (fields.Count == 0)
//         {
//             // Special case
//             builder.JsonAppendNameValue("Type", valueType);
//         }
//         else
//         {
//             builder.EnumerateAndDelimit(fields,
//                 (tb, field) =>
//                 {
//                     object? fieldValue = field.GetValue(value);
//                     tb.JsonAppendNameValue(field.Name, fieldValue);
//                 },
//                 tb => tb.Append(',').NewLine());
//         }
//
//         builder
//             .RemoveIndent()
//             .NewLine()
//             .Append('}');
//         return builder;
//     }
//     
//     private static IndentTextBuilder JsonAppendNameValue(this IndentTextBuilder builder,
//         string? name, object? value)
//     {
//         builder.IfNotEmpty(name, static (tb, n) => tb.Append('"').Append(n).Append("\": "));
//         switch (value)
//         {
//             case null:
//                 return builder.Append("null");
//             case string str:
//                 return builder.Append('"').Append(str).Append('"');
//             case bool boolean:
//                 return builder.Append(boolean ? "true" : "false");
//             case Guid guid:
//                 return builder.Append('"').Append(guid, "D").Append('"');
//             case Array array:
//                 return JsonAppendArray(builder, array);
//             case Type type:
//                 return builder.Append('"').AppendType(type).Append('"');
//             default:
//             {
//                 if (IsNumber(value))
//                 {
//                     return builder.Append(value, "D");
//                 }
//                 else if (IsFloatingPoint(value))
//                 {
//                     return builder.Append(value, "F");
//                 }
//                 else
//                 {
//                     Type valueType = value.GetType();
//                     if (valueType.IsPrimitive || valueType.IsUnmanaged())
//                         Debugger.Break();
//                     return JsonAppendComplex(builder, value);
//                 }
//             }
//         }
//     }
//
//     public static string Render<T>(T? value)
//     {
//         return IndentTextBuilder.New
//             .JsonAppendNameValue(null, value)
//             .ToStringAndDispose();
//     }
// }