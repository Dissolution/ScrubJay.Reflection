//using ScrubJay.Reflection.Naming;
//
//namespace ScrubJay.Reflection.Dumping;
//
//public static class Dump
//{
//    private static TB DumpComplexValue<TB>(this TB builder, Type valueType, object value)
//        where TB : FluentIndentTextBuilder<TB>
//    {
//        if (!valueType.IsClass)
//            Debugger.Break();
//
//        return DumpProperties(builder, valueType, value);
//    }
//
//    private static TB DumpProperties<TB>(this TB builder, Type valueType, object value)
//        where TB : FluentIndentTextBuilder<TB>
//    {
//        // public instance members
//        var members = valueType
//            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
//            .OfType<PropertyInfo>()
//            .ToList();
//
//        return builder
//            .AppendType(valueType)
//            .Append(": ")
//            .Append(value)
//            .If(members.Count > 0, tb => tb
//                .Block("    ", membersBlock => membersBlock
//                    .Delimit(static b => b.NewLine(),
//                        members,
//                        (b, prop) =>
//                        {
//                            var pValue = Result.TryInvoke(() => prop.GetValue(value)).Match<object?>(ok => ok, ex => ex);
//                            b.AppendProperty(prop).Append(": ").Append(pValue);
//                        })));
//    }
//
//    private static TB DumpValue<TB>(this TB builder, object? value, bool allowComplex = true)
//        where TB : FluentIndentTextBuilder<TB>
//    {
//        if (value is null)
//            return builder.Append("null");
//        Type valueType = value.GetType();
//
//        // Have to check for Enum here, as its TypeCode is its underlying Type
//        if (valueType.IsEnum)
//        {
//            var enumName = Enum.GetName(valueType, value);
//            return builder.Append(valueType.NameOf()).Append('.').Append(enumName);
//        }
//
//        var typeCode = Type.GetTypeCode(valueType);
//        switch (typeCode)
//        {
//            case TypeCode.Empty:
//                return builder.Append("null");
//            case TypeCode.DBNull:
//                return builder.Append("DBNull");
//            case TypeCode.Boolean:
//                bool boolean = Notsafe.Unbox<bool>(value!);
//                return builder.Append(boolean ? "true" : "false");
//            case TypeCode.Char:
//                char ch = Notsafe.Unbox<char>(value!);
//                return builder.Append('\'').Append(ch).Append('\'');
//            case TypeCode.SByte:
//                sbyte sb = Notsafe.Unbox<sbyte>(value!);
//                return builder
//                    .Append('(')
//                    .Append(valueType!.NameOf())
//                    .Append(')')
//                    .Append(sb);
//            case TypeCode.Byte:
//                byte b = Notsafe.Unbox<byte>(value!);
//                return builder
//                    .Append('(')
//                    .Append(valueType!.NameOf())
//                    .Append(')')
//                    .Append(b);
//            case TypeCode.Int16:
//                short s = Notsafe.Unbox<short>(value!);
//                return builder
//                    .Append('(')
//                    .Append(valueType!.NameOf())
//                    .Append(')')
//                    .Append(s);
//            case TypeCode.UInt16:
//                ushort us = Notsafe.Unbox<ushort>(value!);
//                return builder
//                    .Append('(')
//                    .Append(valueType!.NameOf())
//                    .Append(')')
//                    .Append(us);
//            case TypeCode.Int32:
//                int i = Notsafe.Unbox<int>(value!);
//                return builder.Append<int>(i);
//            case TypeCode.UInt32:
//                uint ui = Notsafe.Unbox<uint>(value!);
//                return builder.Append(ui).Append('U');
//            case TypeCode.Int64:
//                long l = Notsafe.Unbox<long>(value!);
//                return builder.Append(l).Append('L');
//            case TypeCode.UInt64:
//                ulong ul = Notsafe.Unbox<ulong>(value!);
//                return builder.Append(ul).Append("UL");
//            case TypeCode.Single:
//                float f = Notsafe.Unbox<float>(value!);
//                return builder.Append(f, "N").Append('f');
//            case TypeCode.Double:
//                double d = Notsafe.Unbox<double>(value!);
//                return builder.Append(d, "N").Append('m');
//            case TypeCode.Decimal:
//                decimal m = Notsafe.Unbox<decimal>(value!);
//                return builder.Append(m, "N").Append('m');
//            case TypeCode.DateTime:
//                DateTime dt = Notsafe.Unbox<DateTime>(value!);
//                return builder.Append(dt, "yyyy-MM-dd HH:mm:ss");
//            case TypeCode.String:
//                string str = Notsafe.CastClass<string>(value!);
//                return builder.Append('"').Append(str).Append('"');
//            case TypeCode.Object:
//            default:
//            {
//                break;
//            }
//        }
//
//        if (value is TimeSpan timeSpan)
//        {
//            return builder.Append(timeSpan, "c");
//        }
//        else if (value is Guid guid)
//        {
//            return builder.Append(guid, "D");
//        }
//        else if (value is Type type)
//        {
//            return builder.Append(type.NameOf());
//        }
//        else if (value is MemberInfo member)
//        {
//            return builder.AppendMember(member);
//        }
//        else if (value is Exception ex)
//        {
//            return builder.DumpProperties(valueType, ex);
//        }
//        else if (value is IList list)
//        {
//            return builder.Append('[').Delimit(", ", list.OfType<object?>(), static (b, i) => DumpValue(b, i)).Append(']');
//        }
//#if !NETSTANDARD2_0
//        else if (value is ITuple tuple)
//        {
//            return builder.Append('(')
//                .Delimit(", ", Enumerable.Range(0, tuple.Length), (b, i) => b.DumpValue(tuple[i]))
//                .Append(')');
//        }
//#endif
//
//        if (allowComplex)
//            return DumpComplexValue(builder, valueType, value);
//        return builder.Append(value);
//    }
//
//    public static string Value<T>(T? value)
//    {
//        using var code = new IndentTextBuilder();
//        code.DumpValue(value);
//        return code.ToString();
//    }
//}
