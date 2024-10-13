using System.Dynamic;
using ScrubJay.Text.Builders;

namespace ScrubJay.Reflection.Utilities;

public static class TypeNames
{
    private static readonly ConcurrentTypeMap<string> _typeNameCache;

    static TypeNames()
    {
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
        _typeNameCache = new()
        {
            // C# type aliases
            [typeof(bool)] = "bool",
            [typeof(char)] = "char",
            [typeof(sbyte)] = "sbyte",
            [typeof(byte)] = "byte",
            [typeof(short)] = "short",
            [typeof(ushort)] = "ushort",
            [typeof(int)] = "int",
            [typeof(uint)] = "uint",
            [typeof(long)] = "long",
            [typeof(ulong)] = "ulong",
            [typeof(float)] = "float",
            [typeof(double)] = "double",
            [typeof(decimal)] = "decimal",
            [typeof(string)] = "string",
            [typeof(object)] = "object",
            [typeof(void)] = "void",
            [typeof(nint)] = "nint",
            [typeof(nuint)] = "nuint",
            [typeof(ExpandoObject)] = "dynamic",
        };
        
        InterpolatedText.AddFormatter<Type>(TypeFormatter);
    }

    private static void TypeFormatter(ref InterpolatedText text, Type type, ReadOnlySpan<char> _)
    {
        text.AppendLiteral(NameOf(type));
    }

    private static TextBuilder AppendTypeName(this TextBuilder nameBuilder, Type? type)
    {
        if (type is null)
        {
            return nameBuilder.Append("null");
        }
        
        // Enum types are their NameFrom
        if (type.IsEnum)
        {
            return nameBuilder.Append(type.Name);
        }
        
        // Nullable<T> => T?
        Type? underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            // c# Nullable alias
            return nameBuilder.AppendTypeName(underType).Append('?');
        }

        // Pointer -> utype*
        if (type.IsPointer)
        {
            underType = type.GetElementType().ThrowIfNull();
            return nameBuilder.AppendTypeName(underType).Append('*');
        }

        // ByRef => ref type
        if (type.IsByRef)
        {
            underType = type.GetElementType().ThrowIfNull();
            return nameBuilder.Append("ref ").AppendTypeName(underType);
        }

        // Array => T[]
        if (type.IsArray)
        {
            underType = type.GetElementType().ThrowIfNull();
            return nameBuilder.AppendTypeName(underType).Append("[]");
        }

        // Nested Type?
        if (type.IsNested && !type.IsGenericParameter)
        {
            nameBuilder.AppendTypeName(type.DeclaringType)
                .Append('.');
        }

        // If non-generic
        if (!type.IsGenericType)
        {
            // Just write the type name and we're done
            return nameBuilder.Append(type.Name);
        }

        // Start processing type name
        ReadOnlySpan<char> typeName = type.Name.AsSpan();

        // I'm a parameter?
        if (type.IsGenericParameter)
        {
            var constraints = type.GetGenericParameterConstraints();
            if (constraints.Length > 0)
            {
                nameBuilder.Append(" : ");
                throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }
        
        /* The default NameFrom for a generic type is:
         * Thing<>   = Thing`1
         * Thing<,>  = Thing`2
         * Thing<,,> = Thing`3
         * ...
         */
        var i = typeName.IndexOf('`');
        if (i >= 0)
        {
            nameBuilder.Append(typeName[..i]);
        }
        else
        {
            Debugger.Break();
            nameBuilder.Append(typeName);
        }
        
        // Add our generic types to finish
        var argTypes = type.GetGenericArguments();
        Debug.Assert(argTypes.Length > 0);
        
        return nameBuilder.Append('<')
            .Delimit(", ", argTypes, static (nb, t) => nb.AppendTypeName(t))
            .Append('>');
    }

    private static string CreateTypeName(Type? type)
    {
        return AppendTypeName(new TextBuilder(), type).ToStringAndDispose();
    }
    
    public static string NameOf(this Type? type)
    {
        if (type is null)
            return "null";
        return _typeNameCache.GetOrAdd(type, static t => CreateTypeName(t));
    }

    public static string NameOf<T>() => _typeNameCache.GetOrAdd<T>(static t => CreateTypeName(t));
}