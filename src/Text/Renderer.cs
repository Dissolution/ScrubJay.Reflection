using ScrubJay.Reflection.Collections;

#if NETFRAMEWORK || NETSTANDARD2_0
using Polyfills;
#endif

namespace ScrubJay.Reflection.Text;

public interface IRenderable
{
    void RenderTo(TextBuilder builder);
}

public static class Renderer
{
    public delegate TextBuilder AppendRenderedValue<in T>(TextBuilder builder, T? value);

    private static readonly DelegateMap _delegateMap = new();
    
    
    public static void Map<T>(AppendRenderedValue<T> renderValueTo) 
        => _delegateMap.Add(renderValueTo);

    public static TextBuilder Render<T>(this TextBuilder builder, T? value)
    {
        if (value is IRenderable)
        {
            ((IRenderable)value).RenderTo(builder);
            return builder;
        }
        
        if (_delegateMap.TryGet<AppendRenderedValue<T>>(out var renderer))
        {
            return renderer(builder, value);
        }
        
        switch (value)
        {
            case null:
                return builder.Append("`null`");
            case bool b:
                return builder.AppendIf(b, bool.TrueString, bool.FalseString);
            case byte u8:
                return builder.Append("(byte)").Append(u8);
            case sbyte i8:
                return builder.Append("(sbyte)").Append(i8);
            case short i16:
                return builder.Append("(short)").Append(i16);
            case ushort u16:
                return builder.Append("(ushort)").Append(u16);
            case int i32:
                return builder.Append(i32);
            case uint u32:
                return builder.Append(u32).Append('U');
            case long i64:
                return builder.Append(i64).Append('L');
            case ulong u64:
                return builder.Append(u64).Append("UL");
            case float f32:
                return builder.Append(f32, "G1").Append('f');
            case double f64:
                return builder.Append(f64, "G1").Append('d');
            case decimal dec:
                return builder.Append(dec, "G1").Append('m');
            case TimeSpan ts:
                return builder.Append(ts, "g");
            case DateTime dt:
                return builder.Append(dt, "yyyy/MM/dd HH:mm:ss");
            case Guid guid:
            {
                return builder.Allocate(32, span =>
                {
                    bool wrote = guid.TryFormat(span, out int cw);
                    Debug.Assert(wrote);
                    Debug.Assert(cw == 32);
                    span.ForEach((ref char ch) => ch = char.ToUpper(ch));
                });
            }
            case char ch:
                return builder.Append('\'').Append(ch).Append('\'');
            case string str:
                return builder.Append('"').Append(str).Append('"');
            case ParameterInfo parameter:
                return builder.AppendParameter(parameter);
            case MemberInfo member:
                return builder.AppendMember(member);
            case LocalVariableInfo local:
            {
                return builder
                    .Append("arg")
                    .Append(local.LocalIndex)
                    .AppendIf(local.IsPinned, "📍")
                    .Append(": ")
                    .Render(local.LocalType);
            }
            default:
            {
                string str = value.ToString();
                Debugger.Break();
                return builder.Append(str);
            }
        }
    }
}