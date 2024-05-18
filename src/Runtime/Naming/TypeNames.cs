using System.Diagnostics;
using ScrubJay.Collections;
using ScrubJay.Validation;

namespace ScrubJay.Reflection.Runtime.Naming;

public static class TypeNames
{
    private static readonly ConcurrentTypeMap<string> _cache;

    static TypeNames()
    {
        _cache = new()
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
        };
    }

    private static string GetTypeName(Type type)
    {
        // Enum types are their Name
        if (type.IsEnum)
        {
            return type.Name;
        }

        using var name = new InterpolateDeeper();
        
        // Nullable<T> => T?
        Type? underType = Nullable.GetUnderlyingType(type);
        if (underType is not null)
        {
            // c# Nullable alias
            name.AppendFormatted(underType);
            name.AppendLiteral('?');
            return name.ToString();
        }

        // Pointer -> utype*
        if (type.IsPointer)
        {
            underType = type.GetElementType().ThrowIfNull();
            name.AppendFormatted(underType);
            name.AppendLiteral('*');
            return name.ToString();
        }

        // ByRef => ref type
        if (type.IsByRef)
        {
            underType = type.GetElementType().ThrowIfNull();
            name.AppendLiteral("ref ");
            name.AppendFormatted(underType);
            return name.ToString();
        }

        // Array => T[]
        if (type.IsArray)
        {
            underType = type.GetElementType().ThrowIfNull();
            name.AppendFormatted(underType);
            name.AppendLiteral("[]");
            return name.ToString();
        }

        // Nested Type?
        if (type.IsNested && !type.IsGenericParameter)
        {
            name.AppendFormatted(type.DeclaringType);
            name.AppendLiteral('.');
        }

        // If non-generic
        if (!type.IsGenericType)
        {
            // Just write the type name and we're done
            name.AppendLiteral(type.Name);
            return name.ToString();
        }

        // Start processing type name
        ReadOnlySpan<char> typeName = type.Name.AsSpan();

        // I'm a parameter?
        if (type.IsGenericParameter)
        {
            var constraints = type.GetGenericParameterConstraints();
            if (constraints.Length > 0)
            {
                name.AppendLiteral(" : ");
                Debugger.Break();
            }
            Debugger.Break();
        }
        
        // Name is often something like NAME`2 for NAME<,>, so we want to strip that off
        var i = typeName.IndexOf('`');
        if (i >= 0)
        {
            name.AppendLiteral(typeName[..i]);
        }
        else
        {
            name.AppendLiteral(typeName);
        }
        
        // Add our generic types
        name.AppendLiteral('<');
        var argTypes = type.GetGenericArguments();
        Debug.Assert(argTypes.Length > 0);
        // the first
        name.AppendFormatted(argTypes[0]);
        // the rest are delimited
        for (i = 1; i < argTypes.Length; i++)
        {
            name.AppendLiteral(", ");
            name.AppendFormatted(argTypes[1]);
        }
        name.AppendLiteral('>');

        return name.ToString();
    }
    
    public static string Dump(this Type? type)
    {
        if (type is null)
            return "null";
        return _cache.GetOrAdd(type, GetTypeName);
    }

    public static string Dump<T>() => Dump(typeof(T));
}