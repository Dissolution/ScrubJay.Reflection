using System.Globalization;
using Microsoft.CodeAnalysis.CSharp;
using ScrubJay.Extensions;
using ScrubJay.Text;

namespace ScrubJay.Reflection.Runtime.Naming;

/// <summary>
/// Methods to assist with the naming of <see cref="dynamic"/> and Runtime members
/// </summary>
/// <see href="https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names"/>
/// <see href="https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name"/>
public static class NameHelper
{
    private const char REPLACE_CHAR = '_';
    private static long _nameCount;

#if NET481 || NETSTANDARD2_0_OR_GREATER
    public static HashSet<string> Keywords { get; }
#else
    public static IReadOnlySet<string> Keywords { get; }
#endif

    static NameHelper()
    {
        _nameCount = 0L;
        Keywords = SyntaxFacts.GetKeywordKinds()
            .Select(static kind => SyntaxFacts.GetText(kind))
            .ToHashSet();
    }

    public static bool IsKeyword(string? str)
    {
        return str is not null && Keywords.Contains(str);
    }




    /// <summary>
    /// Is the given <paramref name="ch"/> a valid <see cref="MemberInfo"/> Name <see cref="char"/>?
    /// </summary>
    /// <param name="ch">The <see cref="char"/> to validate</param>
    /// <param name="firstChar"><i>optional</i>, defaults to <c>false</c><br/>
    /// Whether or not you're validating the first or subsequent character in a Member Name</param>
    /// <returns>
    /// <c>true</c> if <paramref name="ch"/> is valid; otherwise <c>false</c>
    /// </returns>
    public static bool IsValidMemberNameCharacter(char ch, bool firstChar = false)
    {
        // Fast allow underscore always
        // '_'.UnicodeCategory = ConnectorPunctuation
        if (ch == '_') return true;

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
        if (firstChar) return false;

        // Remaining allowed characters
        return category is UnicodeCategory.NonSpacingMark
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
        if (name is null) return false;
        if (Keywords.Contains(name)) return false;
        var len = name.Length;
        if (len == 0) return false;
        char ch = name[0];
        if (!IsValidMemberNameCharacter(ch, true)) return false;
        for (var i = 1; i < len; i++)
        {
            if (!IsValidMemberNameCharacter(ch, false)) return false;
        }
        return true;
    }

    public static string CreateMemberName(MemberTypes memberType, string? suggestedName = null)
    {
        ReadOnlySpan<char> name = suggestedName.AsSpan();
        name = name.Trim();
        if (name.Length == 0)
        {
            // No good name passed, return semi-random name
            long nameId = Interlocked.Increment(ref _nameCount);
            return $"{memberType}_{nameId}";
        }

        using var _ = StringBuilderPool.Borrow(out var builder);
        char ch = name[0];
        bool appendedBadChar = false;
        if (!IsValidMemberNameCharacter(ch, true))
        {
            builder.Append('_');
        }
        builder.Append(ch);

        for (var i = 1; i < name.Length; i++)
        {
            ch = name[i];
            if (IsValidMemberNameCharacter(ch))
            {
                builder.Append(ch);
                appendedBadChar = false;
            }
            else if (!appendedBadChar)
            {
                builder.Append(REPLACE_CHAR);
                appendedBadChar = true;
            }
        }
        return builder.ToString();
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
        buffer[1] = char.ToLower(name[0]);
        name.AsSpan(1).CopyTo(buffer[2..]);
        return buffer.ToString();
#else
        return string.Create(property.Name.Length + 1,
            property.Name,
            (span, name) =>
            {
                span[0] = '_';
                span[1] = char.ToLower(name[0]);
                name.AsSpan(1).CopyTo(span[2..]);
            });
#endif
    }

    public static string CreateInterfaceImplementationName(Type interfaceType)
    {
        var interfaceName = interfaceType.Name.TrimStart('I');
        return $"{interfaceName}Impl";
    }
}