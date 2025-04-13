using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.CodeAnalysis.CSharp;

#if NETSTANDARD2_0
using Polyfills;
#endif

namespace ScrubJay.Reflection.Naming;

/// <summary>
/// Utility class for <c>dynamic</c> and Runtime operations
/// </summary>
[PublicAPI]
public static class CodeHelper
{
    // Load all keywords
    private static readonly HashSet<string> _keywords = SyntaxFacts
        .GetKeywordKinds()
        .Select(static kind => SyntaxFacts.GetText(kind))
        .ToHashSet(StringComparer.Ordinal);


    public static bool IsKeyword(string? key)
    {
        return key is not null && _keywords.Contains(key);
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
        return category is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter
            or UnicodeCategory.OtherLetter;
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
        if (_keywords.Contains(name))
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
    public static string GetBackingFieldName(PropertyInfo property)
    {
        // Property => _property
        text name = property.Name.AsSpan();
        int nameLen = name.Length;

        Span<char> buffer = stackalloc char[nameLen + 1];
        buffer[0] = '_';
        buffer[1] = char.ToLower(name[0], CultureInfo.CurrentCulture);
        Notsafe.Text.CopyBlock(name[1..], buffer[2..], nameLen - 1);
        return name.AsString();
    }

    private static string GetNamePart(MemberTypes memberTypes)
    {
        var flags = memberTypes.GetFlags();
        if (flags.Length == 0)
            return "None";
        if (flags.Length == 1)
            return GetDefinedName(memberTypes);

        return TextBuilder.New
            .Delimit('_', flags, static (tb, f) => tb.Append(GetDefinedName(f)))
            .ToString();

        static string GetDefinedName(MemberTypes mt) => mt switch
        {
            MemberTypes.Constructor => "ctor",
            MemberTypes.Event => "event",
            MemberTypes.Field => "field",
            MemberTypes.Method => "method",
            MemberTypes.Property => "property",
            MemberTypes.TypeInfo => "type",
            MemberTypes.Custom => "custom",
            MemberTypes.NestedType => "nested",
            _ => throw InvalidEnumException.Create(mt, "Must be a defined MemberTypes")
        };

    }

    private static readonly ConcurrentDictionary<MemberTypes, ulong> _generatedNameCounts = new();
    private const           char                                     REPLACE_CHAR         = '_';


    public static string GetValidMemberName(MemberTypes memberType, string? suggestion)
    {
        if (suggestion is null)
            goto getNewName;

        // reading from suggestion
        SpanReader<char> reader = new(suggestion.AsSpan());
        if (reader.RemainingCount == 0)
            goto getNewName;
        
        // the name we're building
        Buffer<char> buffer = stackalloc char[reader.RemainingCount];
        SpanReadResult<char> result;
        
        // skip past invalid name characters (not just first name validity)
        result= reader.TryTakeWhile(static ch => !IsValidMemberNameCharacter(ch));
        // if we hit the end, this isn't valid
        if (result.StopReason == StopReason.EndOfSpan || reader.RemainingCount == 0)
            goto getNewName;
        
        // We know we have a first character
        char first = reader.Take();
        
        // If it is not a valid _first_ character
        if (!IsValidMemberNameFirstCharacter(first))
        {
            // we have to preface with our replace
            buffer.Write(REPLACE_CHAR);
        }
        
        // add this char
        buffer.Write(first);

        // now cycle processing
        while (reader.RemainingCount > 0)
        {
            // take any amount of bad characters
            result= reader.TryTakeWhile(static ch => !IsValidMemberNameCharacter(ch));
            
            // if they were all bad, append nothing and be done
            if (result.StopReason == StopReason.EndOfSpan)
                break;
            
            // if we did, replace them with a single REPLACE
            if (result.Span.Length > 0)
            {
                buffer.Write(REPLACE_CHAR);
            }
            
            // now take any amount of good characters
            result = reader.TryTakeWhile(static ch => IsValidMemberNameCharacter(ch));
            
            // write them
            buffer.Write(result.Span);
            
            if (result.StopReason == StopReason.EndOfSpan)
                break;
        }

        string name = buffer.ToString();
        if (_keywords.Contains(name))
        {
            buffer.TryInsert(0, '@').OkOrThrow();
        }

        return buffer.ToStringAndDispose();
        
        // all else failed, generate a new name
        getNewName:
        ulong index = _generatedNameCounts.AddOrUpdate(memberType, 1UL, static (_, count) => count + 1UL);
        return $"{GetNamePart(memberType)}_{index}";
    }

    public static string GetValidMemberName<M>(string? suggestion)
        where M : MemberInfo
    {
        var memberType = typeof(M);
        if (memberType.Implements<ConstructorInfo>())
            return GetValidMemberName(MemberTypes.Constructor, suggestion);
        if (memberType.Implements<EventInfo>())
            return GetValidMemberName(MemberTypes.Event, suggestion);
        if (memberType.Implements<FieldInfo>())
            return GetValidMemberName(MemberTypes.Field, suggestion);
        if (memberType.Implements<MethodInfo>())
            return GetValidMemberName(MemberTypes.Method, suggestion);
        if (memberType.Implements<PropertyInfo>())
            return GetValidMemberName(MemberTypes.Property, suggestion);
        if (memberType.Implements<Type>())
            return GetValidMemberName(MemberTypes.TypeInfo, suggestion);
        return GetValidMemberName(MemberTypes.Custom, suggestion);
    }
}