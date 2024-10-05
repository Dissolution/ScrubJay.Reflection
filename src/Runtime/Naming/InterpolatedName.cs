// using System.Text;
// using ScrubJay.Text;
//
// namespace ScrubJay.Reflection.Runtime.Naming;
//
// [InterpolatedStringHandler]
// public ref struct InterpolateDeeper
// {
//     public static string Resolve(ref InterpolateDeeper interpolation)
//     {
//         return interpolation.ToStringAndDispose();
//     }
//
//     private StringBuilder _stringBuilder;
//
//     public InterpolateDeeper()
//     {
//         _stringBuilder = StringBuilderPool.Rent();
//     }
//
//     public InterpolateDeeper(int literalLength, int formattedCount)
//     {
//         _stringBuilder = StringBuilderPool.Rent();
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void AppendLiteral(char ch)
//     {
//         _stringBuilder.Append(ch);
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void AppendLiteral(string str)
//     {
//         _stringBuilder.Append(str);
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void AppendLiteral(ReadOnlySpan<char> text)
//     {
// #if NET481 || NETSTANDARD2_0
//         unsafe
//         {
//             // `ref readonly char` -> `ref char` -> `void*` -> `char*`
//             char* ptr = (char*)Unsafe.AsPointer<char>(ref Unsafe.AsRef<char>(in text.GetPinnableReference()));
//             // Now we can use Append
//             _stringBuilder.Append(ptr, text.Length);
//         }
// #else
//         _stringBuilder.Append(text);
// #endif
//     }
//
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void AppendLiteral(params char[] characters)
//     {
//         _stringBuilder.Append(characters);
//     }
//
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void AppendFormatted(Type? type)
//     {
//         _stringBuilder.Append(TypeNames.Dump(type));
//     }
//
//     public void AppendFormatted<T>(T? value)
//     {
//         switch (value)
//         {
//             case null:
//                 break;
//             case bool boolean:
//                 _stringBuilder.Append(boolean);
//                 break;
//             case char ch:
//                 _stringBuilder.Append(ch);
//                 break;
//             case sbyte int8:
//                 _stringBuilder.Append(int8);
//                 break;
//             case byte uint8:
//                 _stringBuilder.Append(uint8);
//                 break;
//             case short int16:
//                 _stringBuilder.Append(int16);
//                 break;
//             case ushort uint16:
//                 _stringBuilder.Append(uint16);
//                 break;
//             case int int32:
//                 _stringBuilder.Append(int32);
//                 break;
//             case uint uint32:
//                 _stringBuilder.Append(uint32);
//                 break;
//             case long int64:
//                 _stringBuilder.Append(int64);
//                 break;
//             case ulong uint64:
//                 _stringBuilder.Append(uint64);
//                 break;
//             case float f32:
//                 _stringBuilder.Append(f32);
//                 break;
//             case double f64:
//                 _stringBuilder.Append(f64);
//                 break;
//             case decimal m:
//                 _stringBuilder.Append(m);
//                 break;
//             case char[] chars:
//                 _stringBuilder.Append(chars);
//                 break;
//             case string str:
//                 _stringBuilder.Append(str);
//                 break;
//             default:
//             {
//                 _stringBuilder.Append(value.ToString());
//                 break;
//             }
//         }
//     }
//
//     public void Dispose()
//     {
//         StringBuilder? toReturn = _stringBuilder;
//         _stringBuilder = null!;
//         StringBuilderPool.Return(toReturn);
//     }
//
//     public string ToStringAndDispose()
//     {
//         string str = this.ToString();
//         this.Dispose();
//         return str;
//     }
//
//     public override string ToString() => _stringBuilder?.ToString() ?? throw new ObjectDisposedException(nameof(InterpolateDeeper));
// }