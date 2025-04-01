#pragma warning disable MA0002

using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.CodeAnalysis.CSharp;

#if NETSTANDARD2_0 || NETFRAMEWORK
using Polyfills;
#endif

namespace ScrubJay.Reflection.Naming;

/// <summary>
/// Methods to assist with the naming of <c>dynamic</c> and Runtime members
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names"/>
/// <seealso href="https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name"/>
public static class NameHelper
{
    private const char ReplaceChar = '_';
    private static readonly ConcurrentDictionary<MemberTypes, ulong> _generatedTypeNameCounts = new();

#if NETFRAMEWORK || NETSTANDARD
    public static IReadOnlyCollection<string> Keywords { get; }
#else
    public static IReadOnlySet<string> Keywords { get; }
#endif

    static NameHelper()
    {
        // Load up all known keywords
        Keywords = SyntaxFacts
            .GetKeywordKinds()
            .Select(static kind => SyntaxFacts.GetText(kind))
            .ToHashSet(StringComparer.Ordinal);
    }

    private static ulong MemberCount(MemberTypes memberTypes)
    {
        return _generatedTypeNameCounts.GetValueOrDefault(memberTypes, 0UL);
    }

    private static ulong IncrementMemberCount(MemberTypes memberType)
    {
        return _generatedTypeNameCounts
            .AddOrUpdate(memberType, 1UL, static (_, count) => count + 1);
    }

    /// <summary>
    /// Is the specified <see cref="string"/> a reserved language keyword?
    /// </summary>
    public static bool IsKeyword(string? str)
    {
        return str is not null && Keywords.Contains(str);
    }

    /// <summary>
    /// Is the given <see cref="char"/> valid for use in a <see cref="MemberInfo"/> NameFrom?
    /// </summary>
    /// <param name="ch">
    /// The <see cref="char"/> to validate
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="char"/> is valid for a <see cref="MemberInfo"/> NameFrom; otherwise, <c>false</c>
    /// </returns>
    public static bool IsValidMemberNameFirstCharacter(char ch)
    {
        // Fast allow underscore always
        // '_'.UnicodeCategory = ConnectorPunctuation
        if (ch == '_')
            return true;

        var category = char.GetUnicodeCategory(ch);

        // Always allowed
        if (category is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter
            or UnicodeCategory.OtherLetter)
        {
            return true;
        }

        // No further characters are valid for a first char
        return false;
    }

    /// <summary>
    /// Is the given <see cref="char"/> valid for use in a <see cref="MemberInfo"/> Name?
    /// </summary>
    /// <param name="ch">
    /// The <see cref="char"/> to validate
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="char"/> is valid for a <see cref="MemberInfo"/> Name; otherwise, <c>false</c>
    /// </returns>
    public static bool IsValidMemberNameCharacter(char ch)
    {
        // Fast allow underscore always
        // '_'.UnicodeCategory = ConnectorPunctuation
        if (ch == '_')
            return true;

        var category = char.GetUnicodeCategory(ch);

        // Always allowed
        return category is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter
            or UnicodeCategory.OtherLetter
            or UnicodeCategory.NonSpacingMark
            or UnicodeCategory.SpacingCombiningMark
            or UnicodeCategory.DecimalDigitNumber
            or UnicodeCategory.LetterNumber
            or UnicodeCategory.Format
            or UnicodeCategory.ConnectorPunctuation;
    }

    /// <summary>
    /// Is the given <paramref name="name"/> a valid <see cref="MemberInfo"/> name?
    /// </summary>
    public static bool IsValidMemberName([NotNullWhen(true)] string? name)
    {
        if (name is null)
            return false;
        if (Keywords.Contains(name))
            return false;
        var len = name.Length;
        if (len == 0)
            return false;
        char ch = name[0];
        if (!IsValidMemberNameFirstCharacter(ch))
            return false;
        for (var i = 1; i < len; i++)
        {
            if (!IsValidMemberNameCharacter(ch))
                return false;
        }

        return true;
    }

    public static Option<string> AsValidMemberName(string? name)
    {
        var reader = new SpanReader<char>(name.AsSpan());
        using var writer = new Buffer<char>(reader.RemainingCount);
        bool isValid = false;

        var tookBadChars = reader.TryTakeWhile(ch => !IsValidMemberNameCharacter(ch));
        if (tookBadChars.StopReason == StopReason.EndOfSpan)
            return None();
        if (tookBadChars.Span.Length > 0)
            writer.Add(ReplaceChar);
        var fc = reader.Peek();
        if (IsValidMemberNameFirstCharacter(fc))
        {
            writer.Add(fc);
            reader.Skip();
            isValid = true;
        }

        else
        {
            writer.Add(ReplaceChar);
            if (IsValidMemberNameCharacter(fc))
            {
                reader.Skip();
                writer.Add(fc);
                isValid = true;
            }
        }
        while (reader.RemainingCount > 0)
        {
            tookBadChars = reader.TryTakeWhile(c => !IsValidMemberNameCharacter(c));
            if (tookBadChars.Span.Length > 0)
            {
                writer.Add(ReplaceChar);
            }
            var goodChars = reader.TryTakeWhile(IsValidMemberNameCharacter).Span;
            Debug.Assert(goodChars.Length > 0);
            writer.AddMany(goodChars);
            isValid = true;
        }

        if (!isValid)
            return None();

        name = writer.ToString();
        if (name.Length == 0)
            return None();
        if (Keywords.Contains(name))
            return Some($"@{name}");
        return Some(name);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="namespace"></param>
    /// <returns></returns>
    /// <remarks>
    /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/namespaces#143-namespace-declarations
    /// </remarks>
    public static bool IsValidNamespace(string? @namespace)
    {
        if (@namespace is null)
            return false;
        int len = @namespace.Length;
        if (len == 0)
            return false;
        for (int i = 0; i < len; i++)
        {
            char ch = @namespace[i];
            if (ch != '.' && !IsValidMemberNameFirstCharacter(ch)) // assumed
                return false;
        }
        return true;
    }

    /// <summary>
    /// Creates a backing <see cref="FieldInfo"/> name for a <see cref="PropertyInfo"/>
    /// </summary>
    public static string CreateBackingFieldName(PropertyInfo property)
    {
#if NET481 || NETSTANDARD2_0
        var name = property.Name;
        Span<char> buffer = stackalloc char[name.Length + 1];
        buffer[0] = '_';
        buffer[1] = char.ToLower(name[0], CultureInfo.InvariantCulture);
        Sequence.CopyTo(name.AsSpan(1), buffer.Slice(2));
        return buffer.ToString();
#else
        return string.Create(property.Name.Length + 1,
            property.Name,
            (span, name) =>
            {
                span[0] = '_';
                span[1] = char.ToLower(name[0], CultureInfo.InvariantCulture);
                Sequence.CopyTo(name.AsSpan(1), span.Slice(2));
            });
#endif
    }

    public static string CreateInterfaceImplementationName(Type interfaceType)
    {
        var interfaceName = interfaceType.Name.TrimStart('I');
        return $"{interfaceName}Impl";
    }

    private static string GetMemberTypeNameOrThrow(MemberTypes memberTypes)
        => memberTypes switch
        {
            MemberTypes.Constructor => "ctor",
            MemberTypes.Event => "event",
            MemberTypes.Field => "field",
            MemberTypes.Method => "method",
            MemberTypes.Property => "property",
            MemberTypes.TypeInfo => "type",
            MemberTypes.All => "all",
            MemberTypes.Custom => "custom",
            MemberTypes.NestedType => "nested_type",
            _ => throw new ArgumentOutOfRangeException(nameof(memberTypes), memberTypes, "Only single-flag MemberTypes are supported"),
        };


    public static string MemberName(
        string? suggested,
        MemberTypes memberType = default)
    {
        if (AsValidMemberName(suggested).IsSome(out var name))
            return name;
        string memberTypeName = GetMemberTypeNameOrThrow(memberType);
        ulong count = IncrementMemberCount(memberType);
        return $"{memberTypeName}{count}";
    }

    public static string MethodName(
        string? suggested,
        Type? returnType,
        Type[]? parameterTypes)
    {
        if (AsValidMemberName(suggested).IsSome(out var name))
            return name;

        ulong count = IncrementMemberCount(MemberTypes.Method);
        return TextBuilder.New
            .AppendIf(returnType.IsNullOrVoid(), "action", "func")
            .If(parameterTypes?.Length > 0, b => b.Append('`').Append(parameterTypes!.Length))
            .Append('_')
            .Append(count)
            .ToStringAndDispose();
    }
}
