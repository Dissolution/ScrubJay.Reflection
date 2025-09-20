using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class EventRenderer : Renderer<EventInfo>
{
    public override TextBuilder RenderTo(TextBuilder builder, EventInfo? @event)
    {
        if (@event is null) return builder;

        var (prefix, postfix) = Nullability.Get(@event).GetPrefixPostfix();
        
        return builder
            .IfNotNull(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
            .If(@event.IsStatic(), "static ")
            .Render(@event.EventHandlerType)
            .Append(postfix)
            .Append(' ')
            .Render(@event.OwnerType)
            .Append('.')
            .Append(@event.Name);
    }
}