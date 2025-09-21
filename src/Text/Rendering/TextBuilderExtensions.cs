namespace ScrubJay.Reflection.Text.Rendering;

internal static class TextBuilderExtensions
{
    extension(TextBuilder builder)
    {
        public TextBuilder AppendNameGenericsAndParameters(MemberInfo member)
        {
            string name = member.Name;
            int i = name.LastIndexOf('`');
            if (i >= 0)
            {
                builder.Write(name.AsSpan(0, i));
            }
            else
            {
                builder.Write(name);
            }
            
            var genericTypes = member.GetGenericTypes();
            builder.IfNotEmpty(genericTypes, static (tb, types) => tb
                .Append('<')
                .Delimit(", ", types)
                .Append('>'));

            if (member is PropertyInfo property)
            {
                var indexers = (property.GetIndexParameters());
                if (indexers.Length > 0)
                {
                    builder.Append('(')
                        .Delimit(", ", indexers)
                        .Append(')');
                }
            }
            else if (member is MethodBase method)
            {
                var parameters = method.GetParameters();
                builder.Append('(')
                    .Delimit(", ", parameters)
                    .Append(')');
            }

            return builder;
        }
    }
}