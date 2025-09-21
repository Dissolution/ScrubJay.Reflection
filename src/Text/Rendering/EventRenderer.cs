using ScrubJay.Text.Rendering;

namespace ScrubJay.Reflection.Text.Rendering;

[PublicAPI]
public sealed class EventRenderer : MemberRenderer<EventInfo>
{
    public override TextBuilder RenderTo(TextBuilder builder, EventInfo? @event)
    {
        if (@event is null)
            return builder;

        WriteAttributes(builder, @event, out var typeNullable);

        return builder
            .Render(@event.Visibility)
            .Append(' ')
            .Render(@event.EventHandlerType)
            .If(typeNullable, '?')
            .Append(' ')
            .Append(@event.Name);
    }
}