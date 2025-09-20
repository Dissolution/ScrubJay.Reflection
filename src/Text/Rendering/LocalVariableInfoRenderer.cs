using InlineIL;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public class LocalVariableInfoRenderer : Renderer<LocalVariableInfo>
{
    public override TextBuilder RenderTo(TextBuilder builder, LocalVariableInfo? local)
    {
        if (local is null)
            return builder;

        return builder.Render(local.LocalType)
            .Append(" (")
            .Format(local.LocalIndex)
            .Append(')')
            .If(local.IsPinned, " (pinned)");
    }
}

[PublicAPI]
public sealed class VisibilityRenderer : Renderer<Viz>
{
    public override TextBuilder RenderTo(TextBuilder builder, Viz visibility)
    {
        // private -> public
        bool wrote = false;

        if (visibility.HasFlags(Viz.Private))
        {
            builder.Write("private");
            wrote = true;
        }

        if (visibility.HasFlags(Viz.Protected))
        {
            if (wrote)
                builder.Write(' ');
            builder.Write("protected");
        }

        if (visibility.HasFlags(Viz.Internal))
        {
            if (wrote)
                builder.Write(' ');
            builder.Write("internal");
        }

        if (visibility.HasFlags(Viz.Public))
        {
            if (wrote)
                builder.Write(' ');
            builder.Write("public");
        }

        if (visibility.HasFlags(Viz.Static))
        {
            if (wrote)
                builder.Write(' ');
            builder.Write("static");
        }

        return builder;
    }
}