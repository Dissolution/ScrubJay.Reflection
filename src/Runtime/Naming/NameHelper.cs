#pragma warning disable MA0002

using System.Globalization;
using Microsoft.CodeAnalysis.CSharp;
using ScrubJay.Text.Builders;
#if NETSTANDARD2_0
using Polyfills;
#endif

namespace ScrubJay.Reflection.Runtime.Naming;

/// <summary>
/// Methods to assist with the naming of <c>dynamic</c> and Runtime members
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names"/>
/// <seealso href="https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name"/>
public static class NameHelper
{
    private const char REPLACE_CHAR = '_';
    private static long _nameCount = 0L;

#if NET481 || NETSTANDARD2_0_OR_GREATER
    public static IReadOnlyCollection<string> Keywords { get; }
#else
    public static IReadOnlySet<string> Keywords { get; }
#endif

    static NameHelper()
    {
        // Load up all known keywords
        Keywords = SyntaxFacts.GetKeywordKinds()
            .Select(static kind => SyntaxFacts.GetText(kind))
            .ToHashSet(StringComparer.Ordinal);
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
    /// <param name="firstChar"><i>optional (false)</i><br/>
    /// Is the <see cref="char"/> being validated the first in the NameFrom?
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="char"/> is valid for a <see cref="MemberInfo"/> NameFrom; otherwise, <c>false</c>
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
        if (memberType.FlagCount() != 1)
            throw new ArgumentException("Only one member type may be passed", nameof(memberType));
        
        ReadOnlySpan<char> name = suggestedName.AsSpan().Trim();
        if (name.Length == 0)
        {
            // No good name passed, return semi-random name
            long nameId = Interlocked.Increment(ref _nameCount);
            return $"{memberType}_{nameId}";
        }

        using var builder = new TextBuilder();
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
        buffer[1] = char.ToLower(name[0], CultureInfo.InvariantCulture);
        name.AsSpan(1).CopyTo(buffer[2..]);
        return buffer.ToString();
#else
        return string.Create(property.Name.Length + 1,
            property.Name,
            (span, name) =>
            {
                span[0] = '_';
                span[1] = char.ToLower(name[0], CultureInfo.InvariantCulture);
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